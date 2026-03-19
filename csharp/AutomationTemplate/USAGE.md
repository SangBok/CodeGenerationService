# AutomationTemplate/TestBed 구조

## 프로젝트 구성

- `AutomationTemplate`: 공통 계층(System/Hardware/Utility + 공통 GUI)
- `TestBed`: 설비별 계층(Mechanical/Control/Process + 설비 GUI)

참조 방향은 항상 `TestBed -> AutomationTemplate` 입니다.

## 현재 분리 결과

- 공통 프로젝트(`AutomationTemplate`)에 남아있는 것
  - `0_System`
  - `1_Hardware`
  - `5_Utility`
  - `6_GUI/MainForm` (공통 BT UI)
- 설비 프로젝트(`TestBed`)로 이동한 것
  - `2_Mechanical`
  - `3_Control`
  - `4_Process`
  - `6_GUI/TestBedMainForm` (설비 전용 UI)

## 설비 모듈 확장 방식

설비 프로젝트는 `IEquipmentModule`을 구현해서 공통 시스템에 등록합니다.

- 인터페이스: `AutomationTemplate._0_System.Abstractions.IEquipmentModule`
- 등록 시점: `MSystem.GetInstance().Initialize(new YourEquipmentModule())`

`Register(ContainerBuilder)`에서 설비 레이어를 등록하고,
`WarmUp(IContainer)`에서 초기화가 필요한 서비스 Resolve를 수행합니다.

## 신규 설비 프로젝트 추가 절차

1. 솔루션에 새 프로젝트(예: `NewMachine`)를 생성합니다.
2. `AutomationTemplate` 프로젝트 참조를 추가합니다.
3. `TestBed`의 `0_Bootstrap/TestBedEquipmentProfile.cs`를 기준으로 장비 설정 프로필 클래스를 복사/수정합니다.
4. `config/newmachine-equipment.json`을 만들고 축/실린더/유닛 alias를 설정합니다.
5. `Mechanical/Control/Process` 레이어를 신규 프로젝트에 구현합니다.
6. `IEquipmentModule` 구현체를 작성해 설비 서비스 등록을 구성합니다.
7. `Program.Main`에서 설정 프로필을 로드하고 `MSystem.Initialize(new NewMachineEquipmentModule(profile))`로 초기화합니다.
8. 설비 전용 WinForms UI를 구현하고 필요한 서비스만 `MSystem.Resolve<T>()`로 사용합니다.

## 검증

현재 솔루션은 다음 명령으로 빌드 검증되었습니다.

- `dotnet build AutomationTemplate.sln -c Debug`

## 하드코딩 제거 원칙

- 축 ID, 실린더 ID, BT unit alias는 코드에 직접 쓰지 않고 설비별 `config/*-equipment.json`에서 읽습니다.
- 공통 GUI(`AutomationTemplate/MainForm`)의 축 이벤트 구독 대상은 `hardware.json`의 `axisGroups`에서 동적으로 읽습니다.
- BT 실행 파라미터 기본값은 BT 정의(`bt-tree.json`의 `blackboardSchema.defaultValue`)를 우선 사용합니다.
