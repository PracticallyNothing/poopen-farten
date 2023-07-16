using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathHandle : MonoBehaviour
{
    public void Death() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
