using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpositeDirectioneMoveController : MonoBehaviour
{
    private float _startSwitchPosY = -4f;
    private float _endSwitchPosY = -8f;
    private float _moveSpeed = 16f;
    public IEnumerator MoveToOppositeDirection()
    {
        float currentSwitchPosY = _startSwitchPosY + (_endSwitchPosY - _startSwitchPosY) * 0.25f * Random.Range(0, 5);

        float currentPosY = transform.position.y;
        float offsetY = currentSwitchPosY - currentPosY;
        float waitTime = offsetY / _moveSpeed;
        yield return new WaitForSeconds(waitTime);

        float currentSwitchPosX = transform.position.x;
        float targetSwitchPosX = -currentSwitchPosX;
        float offsetX = targetSwitchPosX - currentSwitchPosX;
        float timeToSwitch = Mathf.Abs(offsetX / _moveSpeed);
        float speedMagnitude = offsetX > 0f ? 1f : -1f;

        float timeElapsed = 0f;
        while (timeElapsed < timeToSwitch)
        {
            timeElapsed += Time.fixedDeltaTime;
            transform.position += speedMagnitude * _moveSpeed * Time.fixedDeltaTime * Vector3.right;
            yield return new WaitForFixedUpdate();
        }

        Vector3 temp = transform.position;
        temp.x = targetSwitchPosX;
        transform.position = temp;
    }
}
