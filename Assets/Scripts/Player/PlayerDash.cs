using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour {

    [SerializeField] private BoolVariable inputDash;
    [SerializeField] private BoolVariable isDashing;
    [SerializeField] private BoolVariable canDash;
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private float dashDuration;
    [SerializeField] private GameEvent OnStartDash;
    [SerializeField] private GameEvent OnFinishDash;
    [SerializeField] private GameEvent OnFinishDashCD;
    [SerializeField] private FloatVariable currentTargetSpeed;
    [SerializeField] private FloatReference dashCooldown;
    [SerializeField] private FloatVariable currentDashCooldown;
    [SerializeField] private FloatReference dashDrag;
    [SerializeField] private FloatReference dashSpeed;

    private Coroutine dashCoroutine;

    public void UpdateDash (Vector2 inputAxis) {
		if (inputDash && canDash)
        {
            dashCoroutine = StartCoroutine(InitiateDash(inputAxis));
        }
	}

    public void InterruptDash()
    {
        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        ResetState();
    }

    private IEnumerator InitiateDash(Vector2 inputAxis)
    {
        OnStartDash.Raise();

        if (inputAxis == Vector2.zero)
            inputAxis = Vector2.up;

        m_Rigidbody.useGravity = false;
        m_Rigidbody.velocity = Vector3.zero;

        canDash.SetValue(false);
        isDashing.SetValue(true);
        m_Rigidbody.drag = dashDrag.Value;
        currentDashCooldown.SetValue(0);

        float currentDashDuration = 0;
        while (currentDashCooldown.Value <= dashDuration)
        {
            m_Rigidbody.velocity = inputAxis * dashSpeed.Value;
            currentDashCooldown.ApplyChange(Time.deltaTime);

            yield return null;
        }

        OnFinishDash.Raise();

        isDashing.SetValue(false);
        m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * currentTargetSpeed.Value;


        while (currentDashCooldown.Value <= dashCooldown)
        {
            currentDashCooldown.ApplyChange(Time.deltaTime);
            yield return null;
        }
        currentDashCooldown.SetValue(0);
        canDash.SetValue(true);

        OnFinishDashCD.Raise();


        yield return 0;
    }



    private void ResetState()
    {
        canDash.SetValue(true);
        isDashing.SetValue(false);
        m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * currentTargetSpeed.Value;
    }
}
