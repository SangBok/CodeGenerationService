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

## 포함된 예제

- `public/examples/move-safe-pos.tree.json`은 레거시 시퀀스를 재현합니다.
  - Z safe → XY move → Z target

## Export한 JSON을 C# 데모로 실행

```bash
cd bt/runtime
dotnet build BtRuntime.sln -c Release
dotnet run -c Release --project .\Bt.Demo\Bt.Demo.csproj -- "D:\path\to\exported.tree.json"
```

