## 1. 개요

- 대상: `09-예시설비-PickPlace-시나리오.md`에 정의된 **단일 Pick&Place 유닛**
- 목적: 예시설비를 5계층 아키텍처(System / Hardware / Mechanical / Control / Process)에 어떻게 분배하는지 정리하여, 코드 생성 시 **레이어 책임과 경계를 명확히** 한다.

---

## 2. 레이어별 책임 요약

| 레이어 | 예시설비에서의 역할 | 주요 요소 예시 |
|--------|----------------------|----------------|
| System | 하드웨어 명세를 설정/상수 형태로 보관 | `DefineAxis`, `DefineIO`, `DefineActuator` 등 |
| Hardware | 실제 축/IO/실린더(그리퍼) 제어 표준 인터페이스 | `HIAxisController`, `HIIoController`, `HICylinder` |
| Mechanical | 유닛 1개 = 1클래스. 기본 단위 동작 구현 | `MHandler` (축 이동, 그립/언그립) |
| Control | Mechanical 유닛들을 조합한 복합 동작 | `CHandler` (SafePos 등 복합 동작) |
| Process | 스텝 기반 프로세스(상태머신) | `PHandler` (HandlerStep) / `PStage` (StageStep) |

---

## 3. System Layer 매핑

- 네임스페이스: `AutomationTemplate._0_System`
- 역할:
  - 하드웨어 명세(JSON 등)를 파싱해 **내부 상수/설정 클래스로 보관**.
  - 축 ID, IO 포트 인덱스, 액추에이터 정의 등을 **enum 또는 상수로 제공**.
- 예시 요소:
  - `0_System/Define/DefineAxis.cs`: 축 이름 enum (`AxisName`) 정의.
  - `0_System/Define/DefineIO.cs`: IO 입력/출력 포트 enum (`IN_MAP`, `OUT_MAP`) 정의.
  - `0_System/Define/DefineActuator.cs`: 실린더/액추에이터 정의(템플릿에 존재).
- 설계 자료 매핑:
  - `03-하드웨어명세-스키마.md`의 `axes`, `ioBoards`, `units` 섹션이 이 레이어의 입력이다.

---

## 4. Hardware Layer 매핑

- 네임스페이스: `AutomationTemplate._1_Hardware`
- 역할:
  - System Layer에서 정의한 ID/구성을 사용해 **표준화된 제어 인터페이스**를 제공.
  - 실제 드라이버/PLC/Fieldbus는 이 레이어 뒤에 숨기고, 상위 레이어는 인터페이스만 본다.
- 예시 요소:
  - 인터페이스
    - `1_Hardware/HIAxisController.cs`: 축 서보/이동/원점/포지션 읽기.
    - `1_Hardware/HIIoController.cs`: IO 입력/출력 on/off 및 상태 확인.
    - `1_Hardware/HICylinder.cs`: 실린더 Forward/Backward 및 상태 확인(그리퍼에 활용).
- 설계 자료 매핑:
  - 하드웨어 명세의 `vendor`, `protocol`, `nodeOrAddress` 등이 실제 구현체 선택에 사용된다.
  - 템플릿은 인터페이스 위주이며, 실제 드라이버 구현체는 프로젝트별로 추가된다.

---

## 5. Mechanical Layer 매핑

- 네임스페이스: `AutomationTemplate._2_Mechanical`
- 역할:
  - Pick&Place 유닛의 **기본 단위 동작**을 제공하는 클래스 구현.
  - Hardware 인터페이스를 주입받아, 축/IO/그리퍼를 조합한 동작을 메서드 단위로 제공.
- 예시 요소:
  - 클래스: `MHandler`
    - `HIAxisController`(X/Y/Z)와 `HICylinder`(그리퍼)를 멤버로 보유.
    - 대표 메서드:
      - `MovePositionZ(int positionidx)`
      - `MovePositionXY(int positionidx)`
      - `MaterialGrip()`, `MaterialUngrip()`
- 설계 자료 매핑:
  - `09-예시설비-PickPlace-시나리오.md`의 유닛–하드웨어 매핑에서 **어떤 축/IO가 이 유닛에 속하는지**를 사용.
  - UML에서는 Mechanical 레벨의 메서드명을 **액티비티/시퀀스의 단위 동작명**으로 대응시킨다.

---

## 6. Control Layer 매핑

- 네임스페이스: `AutomationTemplate._3_Control`
- 역할:
  - Mechanical 레이어의 `PickPlaceUnit`을 사용해 **한 사이클의 흐름**을 정의.
  - 예를 들어, 픽&플레이스 1사이클을 하나의 고수준 동작으로 캡슐화.
- 예시 요소:
  - 클래스: `CHandler`
    - 생성자/멤버로 `MHandler`를 사용한다.
    - 대표 메서드:
      - `MoveSafePos(int positionidx)`:
        - Z축 안전 위치 이동 → XY 이동 → Z축 목표 이동과 같은 **복합 동작**을 제공.
- 설계 자료 매핑:
  - UML 시퀀스/액티비티 다이어그램에서 **Pick&Place 사이클**에 해당하는 시퀀스가 이 레이어의 입력이다.
  - 다이어그램 상의 각 단계는 Mechanical 메서드 호출 순서로 매핑된다.

---

## 7. Process Layer 매핑

- 네임스페이스: `AutomationTemplate._4_Process`
- 역할:
  - 스텝(enum) 기반으로 설비 시퀀스를 진행하는 **상태머신/프로세스**를 담당.
- 예시 요소:
  - 클래스: `PHandler`
    - `HandlerStep` enum을 가지고 `Run()`에서 step 전이를 수행.
  - 클래스: `PStage`
    - `StageStep` enum을 가지고 `Run()`에서 step 전이를 수행.
- 설계 자료 매핑:
  - UML 상태 다이어그램의 상태/전이 이름을 `*Step` enum 및 `Run()` 내부 분기 로직에 반영한다.
  - 시퀀스 다이어그램의 상위 흐름(Operator ↔ 설비 상호작용)이 이 레이어의 시나리오 정의에 대응된다.

---

## 8. 요약

- 예시설비 Pick&Place 유닛은 5계층 구조에 다음과 같이 매핑된다.
  - **System**: `DefineAxis`, `DefineIO`, `DefineActuator` 등 명세 기반 상수/enum.
  - **Hardware**: `HIAxisController`, `HIIoController`, `HICylinder` 등 표준 인터페이스.
  - **Mechanical**: `MHandler` – 기본 단위 동작(축 이동/그립/언그립).
  - **Control**: `CHandler` – 복합 동작(안전 위치 이동 등).
  - **Process**: `PHandler`/`PStage` – 스텝 기반 시퀀스(State Machine).
- 코드 생성 서비스는 이 매핑을 기준으로, 새로운 설비에 대해서도 **동일한 패턴**으로 클래스를 생성하도록 유도할 수 있다.

