# UnityMiniProject
 유니티 미니 프로젝트 점프킹 모작 시도

1. 플레이어의 이동
좌, 우 이동
충전형 점프
- 점프 중 좌우 입력 불가능
- 더블점프는 불가능

벽에 점프하면 튕겨나가게
- wall에 Physics Material 2D 적용 애매
- collision.GetContact(0).normal -> 부딪힌 지점의 벡터에서 impulse로 Addforce한 상태

플레이어가 좌,우 바닥 끄트머리에서 Ground 인식을 못해 이동, 점프를 하지 않는 오류
- BoxCast를 적용해봄

좌우 점프 힘만큼 포물선으로 이동
애매한상태

맵 이동
y값이 맵 가장 위로가면 다음 맵의 가장 아래로 이동
y값이 맵 바닥 밑으로가면 이전 맵의 가장 위로 이동
virtual camera를 구간마다 배치해서 플레이어의 y 위치에 따라 이동하면?



점프킹 에셋, 애니메이션 적용
걷기. 점프 충전, 점프, 추락해서 납작해진,


땅
미끄러지는 얼음 땅
점프로만 이동 가능한 눈 땅
경사짐 - 이동불가능, 쭉 미끄러져 내려가는



바람이 부는 맵
점프할때 좌우 이동이 커지게

