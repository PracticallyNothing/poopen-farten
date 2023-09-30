using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D), typeof(ParticleSystem))]
public class ThrowableAreaOfEffect : MonoBehaviour {
    /// What element this throwable actually represents.
    [SerializeField] Element element;

    /// What effects the throwable causes on hit.
    [SerializeField] Stim stim;

    /// How long the area's effects will be applied for.
    [SerializeField] float durationSeconds = 5;

    private BoxCollider2D boxCollider2D = null;
    private new ParticleSystem particleSystem = null;

    void OnEnable() {
        particleSystem = GetComponent<ParticleSystem>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        boxCollider2D.isTrigger = true;
    }

    void OnValidate() {
        if(particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        var mn = particleSystem.main;
        mn.duration = durationSeconds;
    }

    void OnTriggerEnter2D(Collider2D other) {
        StimResponder stimResponder = other.gameObject.GetComponent<StimResponder>();
        if(stimResponder != null)
            stimResponder.ReactToStim(element, stim);
    }

    private DateTime start;

    void Start() {
        particleSystem = GetComponent<ParticleSystem>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        start = DateTime.Now;

        Destroy(
            gameObject,
            particleSystem.main.duration + particleSystem.main.startLifetime.constant);
    }

    void FixedUpdate() {
        var diff = DateTime.Now - start;

        if(diff.Seconds >= particleSystem.main.duration) {
            boxCollider2D.enabled = false;
        }
    }
}
