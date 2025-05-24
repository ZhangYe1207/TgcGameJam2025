using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProjectDatabase", menuName = "Game/Project Database")]
public class ProjectDatabase : ScriptableObject
{
    public List<Project> allProjects;

    private void OnEnable()
    {
        InitProjects();
    }

    public Project GetProjectByID(string id) {
        return allProjects.Find(p => p.projectId == id);
    }

    private void InitProjects() {
        foreach (var p in allProjects) {
            Debug.Log(p.projectId);
        }
    }
}