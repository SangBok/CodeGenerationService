## 1. 개요

- 대상: `09-예시설비-PickPlace-시나리오.md`에 정의된 **단일 Pick&Place 유닛**
- 목적: 예시설비를 5계층 아키텍처(System / Hardware / Mechanical / Control / Process)에 어떻게 분배하는지 정리하여, 코드 생성 시 **레이어 책임과 경계를 명확히** 한다.

---

## 2. 레이어별 책임 요약

| 레이어 | 예시설비에서의 역할 | 주요 요소 예시 |
|--------|----------------------|----------------|
| System | 하드웨어 명세를 설정/상수 형태로 보관 | 축/IO/유닛 ID, 채널 번호, 설비 ID 등 |
| Hardware | 실제 축/IO/그리퍼 제어를 위한 표준 인터페이스 | `IAxisController`, `IIoController`, `IGripperController` 등 |
| Mechanical | 유닛 1개 = 1클래스. 기본 단위 동작 구현 | `PickPlaceUnit` – 이동/그립/센서대기 메서드 |
| Control | Mechanical 유닛들을 조합한 복합 동작 | `PickPlaceCycle` – 한 사이클(픽→플레이스) 시퀀스 |
| Process | 전체 프로세스/상태머신, 반복/정지 로직 | `PickPlaceProcess`, `ProcessState` enum |

---

## 3. System Layer 매핑

- 네임스페이스: `SampleEquipment.System`
- 역할:
  - 하드웨어 명세(JSON 등)를 파싱해 **내부 상수/설정 클래스로 보관**.
  - 축 ID (`AxisX`, `AxisY`, `AxisZ`), IO 이름(`StartButton`, `PartPresentSensor` 등)을 **타입 안전한 enum 또는 상수로 제공**.
- 예시 요소:
  - `HardwareIds`: 축/IO/유닛의 문자열 ID를 상수/enum으로 정의.
  - `AxisConfig`: 각 축의 모터 타입, 노드 주소, 이동 속도/가속도 기본값 등을 프로퍼티로 보관.
  - `IoConfig`: IO 보드 ID, 채널 인덱스, 방향 등을 보관.
- 설계 자료 매핑:
  - `03-하드웨어명세-스키마.md`의 `axes`, `ioBoards`, `units` 섹션이 이 레이어의 입력이다.

---

## 4. Hardware Layer 매핑

- 네임스페이스: `SampleEquipment.Hardware`
- 역할:
  - System Layer에서 정의한 ID/구성을 사용해 **표준화된 제어 인터페이스**를 제공.
  - 실제 드라이버/PLC/Fieldbus는 이 레이어 뒤에 숨기고, 상위 레이어는 인터페이스만 본다.
- 예시 요소:
  - 인터페이스
    - `IAxisController`: 축 이동/정지/원점복귀 메서드 (`MoveToPositionAsync`, `HomeAsync`, `StopAsync` 등).
    - `IIoController`: 디지털 IO 읽기/쓰기 메서드 (`ReadInput`, `WriteOutput` 등).
    - `IGripperController`: `OpenAsync`, `CloseAsync` 메서드로 그리퍼 제어.
  - 간단 구현 (예: Mock)
    - `MockAxisController`, `MockIoController`, `MockGripperController` 등.
- 설계 자료 매핑:
  - 하드웨어 명세의 `vendor`, `protocol`, `nodeOrAddress` 등이 실제 구현체 선택에 사용된다.
  - 예시설비에서는 구체 드라이버 대신 **데모/Mock 구현**으로 충분하다.

---

## 5. Mechanical Layer 매핑

- 네임스페이스: `SampleEquipment.Mechanical`
- 역할:
  - Pick&Place 유닛의 **기본 단위 동작**을 제공하는 클래스 구현.
  - Hardware 인터페이스를 주입받아, 축/IO/그리퍼를 조합한 동작을 메서드 단위로 제공.
- 예시 요소:
  - 클래스: `PickPlaceUnit`
    - 생성자에서 `IAxisController`, `IGripperController`, `IIoController` 등을 의존성으로 주입.
    - 대표 메서드:
      - `Task MoveToHomeAsync()`
      - `Task MoveToPickPositionAsync()`
      - `Task MoveToPlacePositionAsync()`
      - `Task GripAsync()`
      - `Task ReleaseAsync()`
      - `Task WaitForPartPresentAsync(CancellationToken ct)` 등.
- 설계 자료 매핑:
  - `09-예시설비-PickPlace-시나리오.md`의 유닛–하드웨어 매핑에서 **어떤 축/IO가 이 유닛에 속하는지**를 사용.
  - UML에서는 Mechanical 레벨의 메서드명을 **액티비티/시퀀스의 단위 동작명**으로 대응시킨다.

---

## 6. Control Layer 매핑

- 네임스페이스: `SampleEquipment.Control`
- 역할:
  - Mechanical 레이어의 `PickPlaceUnit`을 사용해 **한 사이클의 흐름**을 정의.
  - 예를 들어, 픽&플레이스 1사이클을 하나의 고수준 동작으로 캡슐화.
- 예시 요소:
  - 클래스: `PickPlaceCycle`
    - 생성자에서 `PickPlaceUnit`을 주입받는다.
    - 대표 메서드:
      - `Task ExecuteCycleAsync(CancellationToken ct)`:
        - `MoveToHomeAsync` → `WaitForPartPresentAsync` → `MoveToPickPositionAsync` → `GripAsync` → `MoveToPlacePositionAsync` → `ReleaseAsync` → `MoveToHomeAsync` 순서로 호출.
- 설계 자료 매핑:
  - UML 시퀀스/액티비티 다이어그램에서 **Pick&Place 사이클**에 해당하는 시퀀스가 이 레이어의 입력이다.
  - 다이어그램 상의 각 단계는 Mechanical 메서드 호출 순서로 매핑된다.

---

## 7. Process Layer 매핑

- 네임스페이스: `SampleEquipment.Process`
- 역할:
  - 설비 전체의 **상태머신**과 메인 루프(스레드)를 담당.
  - Start/Stop/Reset 버튼, 알람 상태, 반복/정지 조건 등을 포함한다.
- 예시 요소:
  - enum: `ProcessState`
    - `Idle`, `Ready`, `Running`, `Alarm`, `Stopping` 등.
  - 클래스: `PickPlaceProcess`
    - 생성자에서 `PickPlaceCycle`, `IIoController` 등을 주입받는다.
    - 메서드:
      - `Task RunAsync(CancellationToken ct)`:
        - `ProcessState`를 기반으로 루프를 돌며, Start/Stop/Reset IO를 감시.
        - `Running` 상태에서는 `PickPlaceCycle.ExecuteCycleAsync`를 반복 호출.
        - 오류/Stop/Reset 조건에 따라 `Alarm` 또는 `Idle`로 전이.
- 설계 자료 매핑:
  - UML 상태 다이어그램의 상태/전이 이름을 `ProcessState` enum 및 메서드명/if 분기 로직에 반영한다.
  - 시퀀스 다이어그램의 상위 흐름(Operator ↔ 설비 상호작용)이 이 레이어의 시나리오 정의에 대응된다.

---

## 8. 요약

- 예시설비 Pick&Place 유닛은 5계층 구조에 다음과 같이 매핑된다.
  - **System**: 명세 기반 상수/설정 – 축/IO/유닛 정의.
  - **Hardware**: 실제 축/IO/그리퍼 제어 인터페이스 및 구현.
  - **Mechanical**: `PickPlaceUnit` – 기본 단위 동작(이동/그립/센서대기).
  - **Control**: `PickPlaceCycle` – 1사이클 흐름 정의.
  - **Process**: `PickPlaceProcess` + `ProcessState` – 설비 전체 상태머신과 메인 루프.
- 코드 생성 서비스는 이 매핑을 기준으로, 새로운 설비에 대해서도 **동일한 패턴**으로 클래스를 생성하도록 유도할 수 있다.

