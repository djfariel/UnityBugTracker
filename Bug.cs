﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System;

[Serializable]
public class Bug {

    public int bugID;
    public string bugDescription;
    public BugSeverity severity;
    public BugState state;
    public bool archived;

    public Bug(int bugID, string bugDescription, BugSeverity severity, BugState state) {
        this.bugID = bugID;
        this.bugDescription = bugDescription;
        this.severity = severity;
        this.state = state;
        this.archived = false;
    }

    public Bug(int bugID, string bugDescription) {
        this.bugID = bugID;
        this.bugDescription = bugDescription;
        this.severity = BugSeverity.Literally_Unplayable;
        this.state = BugState.Pending;
        this.archived = false;
    }

    public enum BugSeverity {
        Literally_Unplayable,
        Visual,
        Minor,
        Major
    }

    public enum BugState {
        Pending,
        Possibly_Fixed,
        Fixed,
        Not_Fixing
    }
}
