# BT Editor (PoC)

웹 기반 **Behavior Tree(BT)** 편집기 PoC입니다. *Mechanical 유닛(Unit)* 제어 동작을 노드로 조립해 **BT JSON**을 만들고, C# 런타임 데모에서 JSON을 **해석/실행**할 수 있습니다.

## 실행

```bash
npm install
npm run dev
```

## 포함된 파일(로딩되는 리소스)

- **스키마**: `public/schemas/behavior-tree.schema.json`
- **노드 카탈로그(PoC)**: `public/schemas/node-catalog.poc.json`
- **예제 트리**: `public/examples/move-safe-pos.tree.json`

## 사용법

자세한 사용법은 `USAGE.md`를 참고하세요.

## Export JSON을 C# 데모로 실행

레포 루트 기준:

```bash
cd bt/runtime
dotnet build BtRuntime.sln -c Release
dotnet run -c Release --project .\Bt.Demo\Bt.Demo.csproj -- "D:\path\to\exported.tree.json"
```
