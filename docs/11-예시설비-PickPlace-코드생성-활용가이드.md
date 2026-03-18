## 1. 목적

- `SampleEquipment.*` 네임스페이스로 구현된 Pick&Place 예시 코드를 **LLM 코드 생성 서비스**에서 Few-shot / 템플릿으로 활용하는 방법을 정리한다.
- 새 설비에 대한 하드웨어 명세 + UML이 주어졌을 때, 이 예시를 참조하여 **동일한 5계층 패턴**의 C# 코드를 생성하도록 유도하는 것이 목표이다.

---

## 2. 예시 코드 구조 요약

- 프로젝트: `csharp/SampleEquipment`
- 타깃: .NET 8 (`SampleEquipment.csproj`)
- 주요 네임스페이스/파일:
  - `SampleEquipment.System`
    - `HardwareIds.cs`
    - `AxisConfig.cs`
    - `IoConfig.cs`
  - `SampleEquipment.Hardware`
    - `Abstractions/IAxisController.cs`
    - `Abstractions/IIoController.cs`
    - `Abstractions/IGripperController.cs`
    - `Mock/MockControllers.cs`
  - `SampleEquipment.Mechanical`
    - `PickPlaceUnit.cs`
  - `SampleEquipment.Control`
    - `PickPlaceCycle.cs`
  - `SampleEquipment.Process`
    - `ProcessState.cs`
    - `PickPlaceProcess.cs`

이 구조 자체가 새로운 설비 코드 생성 시 **레이어/파일 분할의 기준 예시**가 된다.

---

## 3. 하드웨어 명세 → System/Hardware 레이어 Few-shot 활용

### 3.1 System Layer (Step 1)

- 프롬프트에 포함할 것:
  - 새로운 설비의 하드웨어 명세(JSON 조각).
  - 예시설비용 `HardwareIds.cs`, `AxisConfig.cs`, `IoConfig.cs` 코드 일부.
- LLM에게 요구할 것:
  - 설계 자료의 `equipmentId`, `axes`, `ioBoards`, `units`를 참고하여, **동일한 패턴의 System 레이어 코드**를 생성하라고 지시.
  - 새 설비 이름으로 네임스페이스와 클래스명을 조정하되, `HardwareIds.Axes.*`, `HardwareIds.IoInputs.*` 같은 구조는 유지하도록 한다.

### 3.2 Hardware Layer (Step 2)

- 프롬프트에 포함할 것:
  - 새로운 설비의 하드웨어 명세 중 제조사/프로토콜/노드 정보.
  - 예시설비용 `IAxisController`, `IIoController`, `IGripperController`, `MockControllers` 코드 일부.
- LLM에게 요구할 것:
  - `IAxisController` 등 **표준 인터페이스 패턴을 유지**하면서, 새 설비에 맞는 구현 클래스를 생성하게 한다.
  - 실제 드라이버가 없는 경우, 예시처럼 Mock 구현 또는 어댑터 클래스 템플릿을 생성하도록 유도한다.

---

## 4. 유닛 정의 → Mechanical/Control 레이어 Few-shot 활용

### 4.1 Mechanical Layer (Step 3)

- 프롬프트에 포함할 것:
  - 새 설비의 유닛–하드웨어 매핑 (예: `units` 배열).
  - 예시설비용 `PickPlaceUnit.cs` 코드 일부.
- LLM에게 요구할 것:
  - 유닛 1개당 1클래스를 만드는 규칙(예: `{UnitName}Unit`)을 유지.
  - 생성할 클래스에 대해:
    - 생성자에서 필요한 Hardware 인터페이스(축/IO/그리퍼 등)를 주입.
    - 단위 동작 메서드(이동/그립/센서대기 등)를 public 메서드로 정의.
  - 예시설비의 메서드 네이밍 패턴 (`MoveToHomeAsync`, `MoveToPickPositionAsync` 등)을 참고하여, 새 유닛에도 **일관된 네이밍 스타일**을 적용하도록 한다.

### 4.2 Control Layer (Step 4)

- 프롬프트에 포함할 것:
  - 새 설비의 UML 시퀀스/액티비티 다이어그램에서 추출한 복합 시퀀스 정보.
  - 예시설비용 `PickPlaceCycle.cs` 코드 일부.
- LLM에게 요구할 것:
  - Mechanical 유닛 클래스를 조합하여 하나의 사이클/복합 동작을 구현하는 클래스를 생성하라고 지시.
  - 메서드 시그니처 예시: `Task ExecuteCycleAsync(CancellationToken ct)`.
  - UML 상의 단계들을 Mechanical 메서드 호출 순서로 변환하는 패턴을 예시와 동일하게 사용하도록 한다.

---

## 5. 상태/시퀀스 정의 → Process 레이어 Few-shot 활용

### 5.1 Process Layer (Step 5)

- 프롬프트에 포함할 것:
  - 새 설비의 UML 상태 다이어그램에서 추출한 상태/전이 목록.
  - 예시설비용 `ProcessState.cs`, `PickPlaceProcess.cs` 코드 일부.
- LLM에게 요구할 것:
  - 상태 enum (`ProcessState` 유사)과 메인 루프 메서드(`RunAsync`)를 생성하게 한다.
  - Start/Stop/Reset 입력과 알람 상태 처리 패턴을 예시와 유사하게 유지하되, 상태 이름과 전이 조건은 UML 정의에 맞춰 생성하도록 한다.
  - Control 레이어 클래스(예: `{Something}Cycle`)를 사용해, Running 상태에서 실제 사이클을 반복 호출하는 구조를 재사용하게 한다.

---

## 6. 프롬프트 템플릿 예시 (개략)

- 각 Step별 프롬프트에 공통으로 포함:
  - 역할 설명: "당신은 설비 제어 C# 코드를 5계층 구조로 작성하는 전문가입니다. 현재 단계는 [레이어명]입니다."
  - 아키텍처 제약: "System → Hardware → Mechanical → Control → Process 순서를 따르며, 현재 레이어는 하위 레이어만 참조합니다."
  - 출력 형식: "생성할 파일 경로와 전체 C# 코드를 제공하세요."
- 레이어별로:
  - 하드웨어 명세/유닛/UML에서 필요한 조각만 포함.
  - 위에서 정리한 예시설비 코드 일부를 **Few-shot 예시**로 제공.
  - "아래 예시 코드의 구조와 네이밍 패턴을 참고하여, 새 설비용 코드를 생성하세요." 라는 지시를 명시.

---

## 7. 정리

- `SampleEquipment.*` 예시는:
  - 하드웨어 명세 → System/Hardware,
  - 유닛 정의 → Mechanical,
  - 복합 시퀀스 → Control,
  - 상태/시퀀스 → Process
  로 이어지는 전체 흐름을 작은 Pick&Place 설비로 보여준다.
- 코드 생성 서비스에서는 이 예시를 Few-shot/템플릿으로 활용해, 새로운 설비에 대해서도 **동일한 5계층 패턴과 네이밍 규칙**을 유지하도록 LLM을 유도할 수 있다.

