# CodeGenerationService (PoC: Behavior Tree + 웹 편집기)

이 저장소는 레거시 설비 제어 코드 템플릿(`csharp/AutomationTemplate`)과, **Behavior Tree(BT)** 기반으로 *Mechanical 유닛(Unit)* 제어 기능을 **웹 GUI에서 구성**하고 **BT JSON을 런타임에서 해석/실행**하는 PoC를 포함합니다.

## 빠른 시작

### 1) 웹 BT 편집기 실행

```bash
cd web/bt-editor
npm install
npm run dev
```

- 터미널에 표시되는 개발 서버 URL로 접속하세요.
- 편집기는 아래 파일을 로드합니다.
  - 스키마: `public/schemas/behavior-tree.schema.json`
  - 노드 카탈로그: `public/schemas/node-catalog.poc.json`
  - 예제 트리: `public/examples/move-safe-pos.tree.json`

오른쪽 패널에서 **Export**를 눌러 BT JSON을 생성할 수 있습니다.

사용법 문서: `web/bt-editor/USAGE.md`

### 2) BT 런타임 데모 실행(C#)

```bash
cd bt/runtime
dotnet build BtRuntime.sln -c Release
dotnet run -c Release --project .\Bt.Demo\Bt.Demo.csproj
```

Export한 JSON 파일 경로를 넘겨 실행하려면:

```bash
dotnet run -c Release --project .\Bt.Demo\Bt.Demo.csproj -- "D:\path\to\exported.tree.json"
```

## 주요 경로

- BT JSON 스키마: `bt/schema/behavior-tree.schema.json`
- PoC 노드 카탈로그: `bt/schema/node-catalog.poc.json`
- 예제 트리: `bt/examples/move-safe-pos.tree.json`
- 런타임 엔진 + 데모: `bt/runtime/`
- 웹 편집기: `web/bt-editor/`

