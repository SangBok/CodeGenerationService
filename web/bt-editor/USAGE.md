# BT Editor (PoC) — 사용법

이 문서는 *Mechanical 유닛(Unit)* 제어를 위해 **Behavior Tree(BT)** JSON을 만드는 웹 편집기 PoC의 사용법입니다.

## 시작하기

```bash
cd web/bt-editor
npm install
npm run dev
```

터미널에 표시되는 개발 서버 URL로 접속하세요.

## 기본 사용 흐름

### 1) 노드 추가
- 왼쪽 **Palette**에서 노드를 클릭해 캔버스에 추가합니다.

### 2) 노드 연결(부모 → 자식)
- 노드의 아래 핸들에서 다른 노드의 위 핸들로 드래그하면 엣지가 생성됩니다.
- 부모 → 자식 연결은 Export된 JSON에서 `children[]`로 변환됩니다.

### 3) 선택 / 삭제
- 노드를 클릭하면 노드가 선택됩니다.
- 엣지를 클릭하면 엣지가 선택됩니다.
- 빈 캔버스를 클릭하면 선택이 해제됩니다.
- **Delete** 또는 **Backspace**로 현재 선택을 삭제할 수 있습니다.
  - 노드를 삭제하면 연결된 엣지도 함께 삭제됩니다.
  - `input/textarea`에 포커스가 있을 때는 일반적인 텍스트 편집 동작(글자 삭제)으로 동작합니다.

### 4) 노드 파라미터 편집
- 노드를 선택합니다.
- 오른쪽 **Inspector**에서 **Parameters (JSON)**를 편집합니다.
- 파라미터는 블랙보드(Blackboard) 키를 참조할 수 있습니다.

```json
{
  "positionIdx": { "$ref": "positionIdx" }
}
```

### 5) Export / Import
- **Export**를 클릭하면 오른쪽 패널에 BT JSON이 생성됩니다.
  - Export 결과는 `public/schemas/behavior-tree.schema.json`로 검증됩니다.
- 오른쪽 텍스트 박스에 BT JSON을 붙여넣고 **Import**를 클릭하면 다시 캔버스에 로드됩니다.

## 개발자용: 새 노드 추가

런타임(레거시 WinForms .NET Framework)과 웹 편집기 팔레트(Node Catalog)를 함께 확장해야 합니다.

- `docs/06-BT-노드-추가-가이드.md` 참고

## 사용법 예시: Handler 유닛으로 `MoveSafePos` 만들기

레거시 템플릿에서 `CHandler.MoveSafePos()`는 대략 아래 순서였습니다.

- `MovePositionZ(0)` → `MovePositionXY(positionIdx)` → `MovePositionZ(positionIdx)`

웹 편집기에서는 이 “조합”을 BT로 표현합니다.

### A) Handler 유닛(논리 유닛 이름) 정하기

PoC에서는 유닛을 “객체로 추가”하기보다, **노드 파라미터의 `unit` 문자열**로 지정합니다.

- 예시 유닛 이름: `StageHandler`

> 이 이름은 Export된 JSON에 그대로 들어가며, C# 런타임에서는 `IUnitRegistry`가 이 이름으로 유닛 객체를 찾아줍니다.

### B) 블랙보드(Blackboard) 키 준비하기

이 예시에서는 `positionIdx`를 블랙보드 키로 둡니다(나중에 값만 바꿔 재사용 가능).

- 노드 파라미터에서 참조할 때:

```json
{ "$ref": "positionIdx" }
```

### C) 트리 구성(노드 추가/연결)

1. `Sequence` 노드를 1개 추가하고 이름을 `MoveSafePos`로 변경합니다. (이 노드가 루트가 되도록 배치)
2. 아래 액션 노드 3개를 추가합니다.
   - `Unit.AxisMoveAbsolute` (Z safe)
   - `Unit.MoveXYToPosition` (XY)
   - `Unit.AxisMoveAbsolute` (Z target)
3. `Sequence` → (Z safe) → (XY) → (Z target) 순서로 엣지를 연결합니다.
   - 자식 실행 순서는 캔버스에서 **위치(Y)** 기준으로 정렬되어 Export됩니다.

### D) 각 노드 파라미터 입력(Inspector)

1) Z safe 노드(`Unit.AxisMoveAbsolute`)

```json
{
  "unit": "StageHandler",
  "axis": "Z",
  "positionIdx": 0
}
```

2) XY 노드(`Unit.MoveXYToPosition`)

```json
{
  "unit": "StageHandler",
  "positionIdx": { "$ref": "positionIdx" }
}
```

3) Z target 노드(`Unit.AxisMoveAbsolute`)

```json
{
  "unit": "StageHandler",
  "axis": "Z",
  "positionIdx": { "$ref": "positionIdx" }
}
```

### E) Export → C# 런타임에서 실행

1. 편집기에서 **Export**를 눌러 JSON을 생성하고 파일로 저장합니다.
2. C# 데모에서 해당 JSON 경로를 인자로 실행합니다.

## Export한 JSON을 C# 데모로 실행

```bash
cd bt/runtime
dotnet build BtRuntime.sln -c Release
dotnet run -c Release --project .\Bt.Demo\Bt.Demo.csproj -- "D:\path\to\exported.tree.json"
```

