using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

[GlobalConfig("Unibear/Code/Editor/BugTracker/Resources")]
public class BugList : GlobalConfig<BugList> {

    public List<Bug> bugs = new List<Bug>();
    public List<Bug> archivedBugs = new List<Bug>();

}
