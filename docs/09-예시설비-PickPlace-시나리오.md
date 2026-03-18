## 1. 개요

- **예시설비 유형**: 단일 Pick&Place 유닛 (Cartesian 축 3개 + 공압 그리퍼 + 센서 IO)
- **목적**: 03-하드웨어명세-스키마.md 및 04-UML-동작명세-가이드.md를 기준으로, 코드 생성 서비스가 학습/참고할 수 있는 **작은 설비 예시**를 정의한다.
- **사용처**: 레이어드 C# 제어 코드 예시 (SampleEquipment.\*) 생성 및 Few-shot / 프롬프트 예시.

---

## 2. 하드웨어 구성 (개념)

### 2.1 축(Axis)

- **축 구성**
  - X축: 수평 이동 (픽 위치 ↔ 플레이스 위치)
  - Y축: 전후 이동 (작업 영역 접근/이탈)
  - Z축: 상하 이동 (피킹/플레이스 높이 조절)

- **축 특성 요약**
  - 모든 축은 1축 서보 모터 / 동일 제조사 컨트롤러를 사용한다고 가정한다.
  - 각 축은 하드웨어 명세의 `axes` 배열에 1:1로 매핑된다.

```json
{
  "equipmentId": "PickPlaceDemo01",
  "axes": [
    { "id": "AxisX", "name": "PickPlace_X", "type": "linear", "vendor": "DemoServo", "protocol": "EtherCAT", "nodeOrAddress": 1 },
    { "id": "AxisY", "name": "PickPlace_Y", "type": "linear", "vendor": "DemoServo", "protocol": "EtherCAT", "nodeOrAddress": 2 },
    { "id": "AxisZ", "name": "PickPlace_Z", "type": "linear", "vendor": "DemoServo", "protocol": "EtherCAT", "nodeOrAddress": 3 }
  ]
}
```

### 2.2 IO 보드 및 센서

- **IO 보드 구성**
  - 디지털 IO 보드 1장 (`DemoIO-16x16`) 사용.
  - 입력 16채널, 출력 16채널.

- **센서/버튼/실행부 매핑 예시**

```json
{
  "ioBoards": [
    {
      "id": "MainIo",
      "type": "DemoIO-16x16",
      "vendor": "DemoIO",
      "channelCount": 16,
      "channels": [
        { "index": 0, "name": "StartButton", "direction": "input" },
        { "index": 1, "name": "StopButton", "direction": "input" },
        { "index": 2, "name": "ResetButton", "direction": "input" },

        { "index": 3, "name": "PartPresentSensor", "direction": "input" },
        { "index": 4, "name": "PickPositionReached", "direction": "input" },
        { "index": 5, "name": "PlacePositionReached", "direction": "input" },

        { "index": 8, "name": "StartLamp", "direction": "output" },
        { "index": 9, "name": "AlarmLamp", "direction": "output" },
        { "index": 10, "name": "Buzzer", "direction": "output" },

        { "index": 11, "name": "GripperOpenSol", "direction": "output" },
        { "index": 12, "name": "GripperCloseSol", "direction": "output" }
      ]
    }
  ]
}
```

### 2.3 그리퍼

- 공압 그리퍼 1개를 사용하며, IO 출력 2채널로 제어:
  - `GripperOpenSol` (그리퍼 오픈 솔레노이드)
  - `GripperCloseSol` (그리퍼 클로즈 솔레노이드)
- 실제 하드웨어 명세에서는 **그리퍼를 별도 장치 타입**으로 정의할 수도 있고, 단순히 IO 채널 집합으로만 표현할 수도 있다.

---

## 3. 유닛 정의 (Mechanical 관점)

- 유닛 이름: `PickPlaceUnit`
- 역할: 한 개의 부품을 픽 위치에서 집어 들어 플레이스 위치에 내려놓는 단위 동작을 수행.
- 유닛–하드웨어 매핑:
  - 축: AxisX, AxisY, AxisZ
  - IO: StartButton, StopButton, ResetButton, PartPresentSensor, PickPositionReached, PlacePositionReached
  - 그리퍼: GripperOpenSol, GripperCloseSol

예시 유닛–하드웨어 매핑 구조는 다음과 같이 정의할 수 있다.

```json
{
  "units": [
    {
      "id": "PickPlaceUnit",
      "name": "PickPlaceUnit",
      "axes": [ "AxisX", "AxisY", "AxisZ" ],
      "ioInputs": [ "StartButton", "StopButton", "ResetButton", "PartPresentSensor", "PickPositionReached", "PlacePositionReached" ],
      "ioOutputs": [ "StartLamp", "AlarmLamp", "Buzzer", "GripperOpenSol", "GripperCloseSol" ]
    }
  ]
}
```

---

## 4. 동작 시나리오 (텍스트)

### 4.1 기본 Pick&Place 사이클

1. **대기 상태**
   - Start 버튼이 눌리길 대기한다.
   - PartPresentSensor가 ON일 때만 사이클을 시작할 수 있다.
2. **원점/준비 위치 이동**
   - X, Y, Z 축을 안전한 준비 위치로 이동한다.
3. **픽 위치로 이동**
   - X, Y, Z 축을 픽 위치로 이동한다.
   - PickPositionReached 입력(또는 위치 도달 신호)을 확인한다.
4. **그리퍼 클로즈 (픽)**
   - GripperCloseSol ON, GripperOpenSol OFF.
   - 소정 시간 대기 또는 그리퍼 클로즈 센서를 확인한다.
5. **플레이스 위치로 이동**
   - X, Y, Z 축을 플레이스 위치로 이동한다.
   - PlacePositionReached 입력(또는 위치 도달 신호)을 확인한다.
6. **그리퍼 오픈 (플레이스)**
   - GripperOpenSol ON, GripperCloseSol OFF.
   - 소정 시간 대기 후 부품이 내려놓였다고 간주한다.
7. **대기 위치 복귀**
   - X, Y, Z 축을 대기 위치로 이동한다.
8. **사이클 종료 및 반복 여부 결정**
   - PartPresentSensor가 여전히 ON이면 다음 부품 사이클을 준비한다.
   - Stop 버튼이 눌리면 사이클을 중단하고 안전 정지 상태로 전환한다.

### 4.2 오류/인터록 개념

- PartPresentSensor가 OFF인 상태에서 Start 버튼을 누르면 알람을 발생시키고 사이클을 시작하지 않는다.
- 이동 중 Stop 버튼을 누르면 현재 동작을 중지하고 알람 상태로 진입한다.

---

## 5. UML 관점 요약 (개념)

- **상태 다이어그램 (Process Layer 인풋)**
  - 상태 예시: `Idle`, `Ready`, `MoveToPick`, `Pick`, `MoveToPlace`, `Place`, `ReturnToHome`, `Alarm`.
  - 전이 조건: Start 버튼, Stop 버튼, 센서 ON/OFF, 타임아웃 등.
- **시퀀스/액티비티 다이어그램 (Mechanical/Control 인풋)**
  - 액터: `Operator`, `PickPlaceUnit`.
  - 메시지/동작: `MoveToPick()`, `CloseGripper()`, `MoveToPlace()`, `OpenGripper()`, `ReturnToHome()` 등으로 추상화한다.

