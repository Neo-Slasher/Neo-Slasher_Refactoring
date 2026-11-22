# 기여 안내

## 빌드

`Assets/Editor/BuildScript.cs`의 메서드를 Unity 명령줄로 실행합니다. 빌드할 씬은 Build Settings에서 켠 씬입니다.

```powershell
Unity -batchmode -quit -projectPath . -executeMethod BuildScript.BuildAndroidDev   # Builds/NeoSlasher-dev.apk
Unity -batchmode -quit -projectPath . -executeMethod BuildScript.BuildWindowsDev   # Builds/Windows/Neo-Slasher.exe
```

- 둘 다 Development 빌드입니다. Android 빌드는 디버그 서명을 써서 키스토어가 필요 없습니다.
- 빌드 결과가 성공이면 종료 코드 0, 아니면 1로 끝납니다.
- `Builds/`는 git에 넣지 않습니다.

## 씬 정리

`Assets/Editor/MissingScriptCleaner.cs`의 `MissingScriptCleaner.CleanBuildScenes`는 Build Settings에서 켠 씬의 missing script 컴포넌트를 지우고 저장합니다. 위와 같이 `-executeMethod`로 실행합니다.

## 작업 규칙

- `Assets/Scripts`의 C# 파일은 UTF-16 LE(BOM) 인코딩입니다. 고칠 때 인코딩과 원래 줄바꿈을 유지합니다.
