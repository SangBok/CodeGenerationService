# 신규 설비 개발 가이드

## 목적

새 설비를 만들 때 `AutomationTemplate`(공통) 재사용을 최대화하고, 설비별 코드는 별도 프로젝트로만 구현합니다.

## 권장 구조

- `AutomationTemplate` (공통)
  - `0_System`, `1_Hardware`, `5_Utility`, 공통 GUI
- `NewMachine` (설비 전용)
  - `2_Mechanical`, `3_Control`, `4_Process`, 설비 GUI, 설비 bootstrap

참조 방향은 `NewMachine -> AutomationTemplate` 단방향입니다.

## 구현 절차

1. 솔루션에 `NewMachine` 프로젝트를 추가합니다.
2. `AutomationTemplate` 프로젝트 참조를 추가합니다.
3. `TestBed` 프로젝트를 템플릿으로 복사해서 아래 파일을 준비합니다.
   - `0_Bootstrap/NewMachineEquipmentProfile.cs`
   - `0_Bootstrap/NewMachineEquipmentModule.cs`
   - `config/newmachine-equipment.json`
   - `Program.cs`
4. `newmachine-equipment.json`에 설비 자원명을 정의합니다.
   - handler/stage 축 ID
   - 실린더 ID
   - BT unit alias 목록
   - UI 기본 파라미터(예: safePositionIndex)
5. `Mechanical/Control/Process`를 설비 요구사항에 맞게 구현합니다.
6. `NewMachineEquipmentModule`에서 DI 등록을 구성합니다.
   - 축/실린더 resolve는 문자열 상수 대신 `profile` 값 사용
   - BT가 찾는 unit alias는 `profile.UnitAliases`를 순회해 등록
7. `Program.Main`에서 profile 로드 후 시스템 초기화합니다.
   - `var profile = NewMachineEquipmentProfile.LoadFromConfigDirectory(...)`
   - `MSystem.GetInstance().Initialize(new NewMachineEquipmentModule(profile))`
8. 설비 전용 UI를 구현합니다.
   - 공통 BT UI가 필요하면 `new AutomationTemplate.MainForm().Show(...)`로 재사용

## 하드코딩 금지 체크리스트

- 코드에 `"HandlerX"`, `"Gripper"` 같은 자원명 직접 입력 금지
- 코드에 `"StageHandler"` 같은 BT unit alias 직접 입력 금지
- UI 시퀀스 파라미터(예: 위치 인덱스) 직접 숫자 입력 금지
- 모든 가변값은 `config/newmachine-equipment.json` 또는 BT JSON으로 관리

## 빌드/검증

- `dotnet build AutomationTemplate.sln -c Debug`
- 신규 설비 실행 후 확인
  - 시스템 초기화 성공
  - 축 이벤트 로그 정상 출력
  - BT 로드/실행 정상 동작
  - 설비 UI 동작 정상
