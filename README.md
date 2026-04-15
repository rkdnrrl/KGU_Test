라이브 데모: https://pub-db4c1cee4176437eb3be33309fcd49ff.r2.dev/index.html

1. 실행 방법 (빌드 및 로컬 서버 실행)
Unity에서 프로젝트를 열고 WebGL 플랫폼으로 전환합니다.
실행 씬(예: Assets/Scenes/Build.unity)을 Build Settings에 등록한 뒤 WebGL 빌드를 생성합니다.
로컬 테스트는 정적 서버로 실행합니다. (localhost 권장)
배포 시 WebGL 빌드 결과물을 Cloudflare R2에 업로드하고 index.html로 접근합니다.
참고:

마이크 기능은 HTTPS/localhost 환경에서 동작합니다.
iframe 임베드 시 allow="microphone" 설정이 필요합니다.
서버 Permissions-Policy에서 마이크가 차단되지 않아야 합니다.

2. 사용 기술 스택과 선택 이유
Unity (WebGL)
브라우저에서 즉시 실행 가능한 배포 형태가 필요해 WebGL을 선택했습니다.
Google Spreadsheet + JSON 변환
비개발자도 시나리오/대사/액션 데이터를 쉽게 수정할 수 있도록 데이터 기반 구조로 구성했습니다.
OpenAI STT
다국어 음성 입력을 문자열로 받아 시나리오 인터랙션과 연결하기 위해 사용했습니다.
WebGL JS TTS (unitycoder/webgl-js-TTS)
유료 에셋 없이 WebGL에서 TTS를 구현하기 위해 사용했습니다.
Cloudflare R2 Hosting
정적 WebGL 빌드를 간단하고 저렴하게 배포하기 위해 사용했습니다.
AI 도구: ChatGPT, Codex
WebGL 음성 처리/이벤트 구조/파라미터 파싱 등 구현 및 디버깅에 활용했습니다.
애니메이션 =  Animancer 에셋 사용
3. 프로젝트 구조 설명
핵심 폴더:

Assets/Script/Data: 테이블 로딩 및 조회 (TableManager)
Assets/Script/Senario: 시나리오 실행기 (Scenario)
Assets/Script/Handler: 타입별 실행 핸들러 (Dialogue/Animation/Camera/Interaction/Move/Quiz)
Assets/Script/Interaction: 인터랙션 컴포넌트 (Delay, Grab, ListenVoice, Quiz)
Assets/Script/TTS: TTS 래퍼
Assets/Plugins/WebGL: WebGL 브리지 (microphone.jslib, tts.jslib)
실행 흐름:

Scenario가 현재 그룹을 기준으로 Action 목록을 읽고 타입별 핸들러를 호출합니다.
핸들러는 대상 컴포넌트에 이벤트를 전달해 실제 동작(대사, 카메라, 애니메이션, 상호작용)을 실행합니다.
인터랙션 완료 시 다음 그룹으로 진행합니다.

4. 대화/시나리오 데이터 구조 (Google Spreadsheet)
워크시트 구성:

Scenario: 시나리오 루트/진입 정보
Group: 여러 Action을 묶는 실행 단위
Action: 실제 실행 명령(Type/Target/Command/Duration/Params)
Text: 대사/문자열 원문
Animation: 애니메이션 매핑
Camera: 카메라 매핑
Interaction: 상호작용 정의
Quiz: 퀴즈 데이터



스프레드시트 작성
JSON 변환
Unity Resources/Data로 반영
런타임에서 TableManager로 조회

5. WebGL 전환 과정에서 겪은 문제와 해결 방법
Windows DictationRecognizer는 WebGL에서 직접 사용이 어려워 JS 브리지 방식으로 전환했습니다.
마이크 시작은 사용자 클릭 이벤트에서 직접 호출하도록 구성해 브라우저 권한 정책을 충족했습니다.
마이크 권한 차단 문제는 사이트 권한, iframe allow, Permissions-Policy를 점검해 해결했습니다.
STT 요청은 WebGL 환경 제약을 고려해 HTTP 요청 경로와 인증 토큰 입력값을 분리해 처리했습니다.
다국어 입력 안정화를 위해 STT 결과 후처리(정규화) 흐름을 추가했습니다.
6. AI 도구 활용 내역
ChatGPT: 구조 설계 방향 정리, WebGL 음성 권한 이슈 분석, STT/TTS 적용 전략 검토
Codex: Unity C# 코드 수정, 이벤트 연결 구조 개선, Params 파싱/인터랙션 로직 구현, WebGL 연동 디버깅
7. 시간과 돈이 있었다면 더 추가했을 기능

&#x20;    유료에셋을 구매하여 웹만이 아닌 앱, 에디터에서도 사용할수 있는 tts에셋 구매

&#x20;    더 좋은 퀄리티의 UI와 배경작업 및 애니메이션과 맞는 캐릭터

&#x20;    게임 최적화

&#x20;    

