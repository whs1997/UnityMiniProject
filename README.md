# UnityMiniProject
 유니티 미니 프로젝트 점프킹 모작 시도

1. 플레이어의 이동
좌, 우 이동, 점프 (스페이스바)
- 점프 중 좌우 입력 불가능
- 더블점프는 불가능

벽에 점프하면 튕겨나가게
- wall에 Physics Material 2D 적용

플레이어가 좌,우 바닥 끄트머리에서 Ground 인식을 못해 이동, 점프를 하지 않는 오류
- BoxCast를 적용해서 완화

맵 이동
- 플레이어의 y 위치가 화면 밖으로 넘어가면 해당 위치의 카메라로 전환

캐릭터 점프킹 에셋, 애니메이션 적용

맵
- 미끄러지는 얼음 땅
- 점프로만 이동 가능한 눈 땅
- 경사진 땅 - 닿으면 미끄러져내려감

ESC 퍼즈 화면
- 점프 횟수, 엎어진 횟수 표시
- Setting에서 BGM과 SFX의 볼륨 조절 가능
- 점프 키 변경 가능
- Quit 타이틀 씬으로 이동
