using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    public virtual void Enter() { } // 상태 시작 했을 때

    public virtual void Update() { } // 동작 중일 때

    public virtual void Exit() { } // 마무리 될 때
}
