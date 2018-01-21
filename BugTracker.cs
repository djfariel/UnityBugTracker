using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using Sirenix.Utilities.Editor;

public class BugTracker : OdinEditorWindow {


    #region Editor Data Loading and Saving

    #region Misc Data

    BugList bugList;
    List<Bug> visibleList = new List<Bug>();
    Color preColor;
    int numberOfPages;

    public int CurrentPage
    {
        get
        {
            return EditorPrefs.GetInt("displayCurrentPage");
        }

        set
        {
            EditorPrefs.SetInt("displayCurrentPage", value);
        }
    }
    #endregion

    #region Main Tab
    [PropertyOrder(-10)]
    [TabGroup("TopBox", "Main")]
    private void AddBug() {
        bugList.bugs.Add(new Bug(bugList.bugs.Count, ""));
        RecalculateVisibleList();
    }
    #endregion

    #region Display State Options Tab
    [TabGroup("TopBox", "Display State Options"), LabelText("Show Archived Instead")]
    public bool ShowArchived
    {
        get
        {
            return EditorPrefs.GetBool("displayShowArchived");
        }

        set
        {
            EditorPrefs.SetBool("displayShowArchived", value);
            RecalculateVisibleList();
            CurrentPage = 0;
        }
    }

    [TabGroup("TopBox", "Display State Options"), DisableIf("ShowArchived")]
    public bool ShowPending
    {
        get
        {
            return EditorPrefs.GetBool("displayShowPending");
        }

        set
        {
            EditorPrefs.SetBool("displayShowPending", value);
            RecalculateVisibleList();
        }
    }

    [TabGroup("TopBox", "Display State Options"), DisableIf("ShowArchived")]
    public bool ShowPossiblyFixed
    {
        get
        {
            return EditorPrefs.GetBool("displayShowPossiblyFixed");
        }

        set
        {
            EditorPrefs.SetBool("displayShowPossiblyFixed", value);
            RecalculateVisibleList();
        }
    }

    [TabGroup("TopBox", "Display State Options"), DisableIf("ShowArchived")]
    public bool ShowFixed
    {
        get
        {
            return EditorPrefs.GetBool("displayShowFixed");
        }

        set
        {
            EditorPrefs.SetBool("displayShowFixed", value);
            RecalculateVisibleList();
        }
    }

    [TabGroup("TopBox", "Display State Options"), DisableIf("ShowArchived")]
    public bool ShowNotFixing
    {
        get
        {
            return EditorPrefs.GetBool("displayShowNotFixing");
        }

        set
        {
            EditorPrefs.SetBool("displayShowNotFixing", value);
            RecalculateVisibleList();
        }
    }
    #endregion

    #region Display Severity Options Tab
    [TabGroup("TopBox", "Display Severity Options")]
    public bool ShowLiterallyUnplayable
    {
        get
        {
            return EditorPrefs.GetBool("displayShowLiterallyUnplayable");
        }

        set
        {
            EditorPrefs.SetBool("displayShowLiterallyUnplayable", value);
            RecalculateVisibleList();
        }
    }

    [TabGroup("TopBox", "Display Severity Options")]
    public bool ShowVisual
    {
        get
        {
            return EditorPrefs.GetBool("displayShowVisual");
        }

        set
        {
            EditorPrefs.SetBool("displayShowVisual", value);
            RecalculateVisibleList();
        }
    }

    [TabGroup("TopBox", "Display Severity Options")]
    public bool ShowMinor
    {
        get
        {
            return EditorPrefs.GetBool("displayShowMinor");
        }

        set
        {
            EditorPrefs.SetBool("displayShowMinor", value);
            RecalculateVisibleList();
        }
    }

    [TabGroup("TopBox", "Display Severity Options")]
    public bool ShowMajor
    {
        get
        {
            return EditorPrefs.GetBool("displayShowMajor");
        }

        set
        {
            EditorPrefs.SetBool("displayShowMajor", value);
            RecalculateVisibleList();
        }
    }
    #endregion

    #region Settings Tab
    [TabGroup("TopBox", "Settings"), LabelWidth(160)]
    public Color LiterallyUnplayableColor
    {
        get
        {
            return new Color(EditorPrefs.GetFloat("literallyUnplayableColorR"), EditorPrefs.GetFloat("literallyUnplayableColorG"), EditorPrefs.GetFloat("literallyUnplayableColorB"), 1f);
        }

        set
        {
            EditorPrefs.SetFloat("literallyUnplayableColorR", value.r);
            EditorPrefs.SetFloat("literallyUnplayableColorG", value.g);
            EditorPrefs.SetFloat("literallyUnplayableColorB", value.b);
        }
    }

    [TabGroup("TopBox", "Settings"), LabelWidth(160)]
    public Color VisualColor
    {
        get
        {
            return new Color(EditorPrefs.GetFloat("visualColorR"), EditorPrefs.GetFloat("visualColorG"), EditorPrefs.GetFloat("visualColorB"), 1f);
        }

        set
        {
            EditorPrefs.SetFloat("visualColorR", value.r);
            EditorPrefs.SetFloat("visualColorG", value.g);
            EditorPrefs.SetFloat("visualColorB", value.b);
        }
    }

    [TabGroup("TopBox", "Settings"), LabelWidth(160)]
    public Color MinorColor
    {
        get
        {
            return new Color(EditorPrefs.GetFloat("minorColorR"), EditorPrefs.GetFloat("minorColorG"), EditorPrefs.GetFloat("minorColorB"), 1f);
        }

        set
        {
            EditorPrefs.SetFloat("minorColorR", value.r);
            EditorPrefs.SetFloat("minorColorG", value.g);
            EditorPrefs.SetFloat("minorColorB", value.b);
        }
    }

    [TabGroup("TopBox", "Settings"), LabelWidth(160)]
    public Color MajorColor
    {
        get
        {
            return new Color(EditorPrefs.GetFloat("majorColorR"), EditorPrefs.GetFloat("majorColorG"), EditorPrefs.GetFloat("majorColorB"), 1f);
        }

        set
        {
            EditorPrefs.SetFloat("majorColorR", value.r);
            EditorPrefs.SetFloat("majorColorG", value.g);
            EditorPrefs.SetFloat("majorColorB", value.b);
        }
    }

    [TabGroup("TopBox", "Settings"), LabelWidth(160)]
    public Color FixedColor
    {
        get
        {
            return new Color(EditorPrefs.GetFloat("fixedColorR"), EditorPrefs.GetFloat("fixedColorG"), EditorPrefs.GetFloat("fixedColorB"), 1f);
        }

        set
        {
            EditorPrefs.SetFloat("fixedColorR", value.r);
            EditorPrefs.SetFloat("fixedColorG", value.g);
            EditorPrefs.SetFloat("fixedColorB", value.b);
        }
    }

    [TabGroup("TopBox", "Settings"), LabelWidth(160)]
    public Color NotFixingColor
    {
        get
        {
            return new Color(EditorPrefs.GetFloat("notFixingColorR"), EditorPrefs.GetFloat("notFixingColorG"), EditorPrefs.GetFloat("notFixingColorB"), 1f);
        }

        set
        {
            EditorPrefs.SetFloat("notFixingColorR", value.r);
            EditorPrefs.SetFloat("notFixingColorG", value.g);
            EditorPrefs.SetFloat("notFixingColorB", value.b);
        }
    }

    [TabGroup("TopBox", "Settings"), LabelWidth(160)]
    public Color ArchivedColor
    {
        get
        {
            return new Color(EditorPrefs.GetFloat("archivedColorR"), EditorPrefs.GetFloat("archivedColorG"), EditorPrefs.GetFloat("archivedColorB"), 1f);
        }

        set
        {
            EditorPrefs.SetFloat("archivedColorR", value.r);
            EditorPrefs.SetFloat("archivedColorG", value.g);
            EditorPrefs.SetFloat("archivedColorB", value.b);
        }
    }

    [TabGroup("TopBox", "Settings"), LabelWidth(160)]
    public int ItemsToShowPerPage
    {
        get
        {
            return EditorPrefs.GetInt("displayItemsPerPage");
        }

        set
        {
            EditorPrefs.SetInt("displayItemsPerPage", value);
            RecalculateVisibleList();
        }
    }

    [TabGroup("TopBox", "Settings")]
    [Button("Reset Default Values", ButtonSizes.Small)]
    void Initialize() {
        EditorPrefs.SetBool("Initialized", true);
        ShowPending = true;
        ShowPossiblyFixed = true;
        ShowFixed = true;
        ShowNotFixing = true;
        ShowLiterallyUnplayable = true;
        ShowVisual = true;
        ShowMinor = true;
        ShowMajor = true;
        ShowArchived = false;
        LiterallyUnplayableColor = Color.white;
        VisualColor = Color.cyan;
        MinorColor = Color.yellow;
        MajorColor = Color.red;
        FixedColor = Color.green;
        NotFixingColor = Color.magenta;
        ArchivedColor = Color.gray;
        ItemsToShowPerPage = 5;
        RecalculateMaxPages();
    }

    #endregion

    #endregion

    #region Unity Stuff

    [MenuItem("Window/Bug Tracker")]
    private static void OpenWindow() {
        GetWindow(typeof(BugTracker)).Show();
    }

    private void OnEnable() {
        if(!EditorPrefs.GetBool("Initialized")) {
            Initialize();
        }
        bugList = BugList.Instance;
        if(GUI.changed) {
            EditorUtility.SetDirty(BugList.Instance);
        }
    }

    #endregion

    #region Main Editor Display

    [OnInspectorGUI]
    void MainDisplay() {
        preColor = GUI.backgroundColor;

        if(visibleList.Count == 0) {
            GUILayout.Label("There are no bugs here! =)");
        } else {
            ShowLocationSelectors();
            ShowHeaders();
            DisplayByTypeSelected();
            ShowLocationSelectors();
        }

        BugList.Instance.bugs = bugList.bugs;

        if(GUI.changed) {
            EditorUtility.SetDirty(BugList.Instance);
        }
    }

    #endregion

    #region Draw Functions

    void ShowHeaders() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("ID", GUILayout.Width(50));
        GUILayout.Label("Bug Description", GUILayout.MinWidth(300f));
        GUILayout.Label("Severity / State", GUILayout.Width(100f));
        GUILayout.Label("Archived", GUILayout.Width(70));
        GUILayout.EndHorizontal();
    }

    void ShowLocationSelectors() {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("<<-", GUILayout.Width(50))) {
            CurrentPage = 0;
        }
        if(GUILayout.Button("<", GUILayout.Width(50))) {
            CurrentPage--;
            if(CurrentPage < 0)
                CurrentPage = 0;
        }
        GUILayout.FlexibleSpace();
        GUILayout.Label("Page " + (CurrentPage + 1).ToString() + "/" + numberOfPages.ToString(), GUILayout.Width(60));
        GUILayout.FlexibleSpace();
        if(GUILayout.Button(">", GUILayout.Width(50))) {
            CurrentPage++;
            if(CurrentPage == numberOfPages)
                CurrentPage = numberOfPages - 1;
        }
        if(GUILayout.Button("->>", GUILayout.Width(50))) {
            CurrentPage = numberOfPages - 1;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void DisplayByTypeSelected() {
        bool statePass = false;
        bool severityPass = false;
        for(int i = (CurrentPage * ItemsToShowPerPage); i < ((CurrentPage + 1) * ItemsToShowPerPage); i++) {
            if(i >= visibleList.Count)
                return;

            statePass = false;
            severityPass = false;

            if(ShowArchived != visibleList[i].archived)
                continue;

            if(ShowArchived) {
                statePass = true;
            }

            if(ShowPending && visibleList[i].state == Bug.BugState.Pending)
                statePass = true;
            if(ShowPossiblyFixed && visibleList[i].state == Bug.BugState.Possibly_Fixed)
                statePass = true;
            if(ShowFixed && visibleList[i].state == Bug.BugState.Fixed)
                statePass = true;
            if(ShowNotFixing && visibleList[i].state == Bug.BugState.Not_Fixing)
                statePass = true;

            if(ShowLiterallyUnplayable && visibleList[i].severity == Bug.BugSeverity.Literally_Unplayable)
                severityPass = true;
            if(ShowVisual && visibleList[i].severity == Bug.BugSeverity.Visual)
                severityPass = true;
            if(ShowMinor && visibleList[i].severity == Bug.BugSeverity.Minor)
                severityPass = true;
            if(ShowMajor && visibleList[i].severity == Bug.BugSeverity.Major)
                severityPass = true;

            if(statePass && severityPass)
                ShowBug(visibleList[i]);
        }
    }

    void ShowBug(Bug bug) {
        if(bug.state != Bug.BugState.Fixed && bug.state != Bug.BugState.Not_Fixing) {
            if(bug.severity == Bug.BugSeverity.Literally_Unplayable) {
                GUI.backgroundColor = LiterallyUnplayableColor;
            } else if(bug.severity == Bug.BugSeverity.Visual) {
                GUI.backgroundColor = VisualColor;
            } else if(bug.severity == Bug.BugSeverity.Minor) {
                GUI.backgroundColor = MinorColor;
            } else if(bug.severity == Bug.BugSeverity.Major) {
                GUI.backgroundColor = MajorColor;
            }
        } else {
            if(bug.archived) {
                GUI.backgroundColor = ArchivedColor;
            } else {
                if(bug.state == Bug.BugState.Not_Fixing) {
                    GUI.backgroundColor = NotFixingColor;
                } else {
                    GUI.backgroundColor = FixedColor;
                }
            }
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label(bug.bugID.ToString(), GUILayout.Width(50));
        EditorStyles.textField.wordWrap = true;
        bug.bugDescription = EditorGUILayout.TextArea(bug.bugDescription, GUILayout.MinWidth(300f));
        GUILayout.BeginVertical(GUILayout.Width(100f));
        bug.severity = (Bug.BugSeverity)EditorGUILayout.EnumPopup("", bug.severity, GUILayout.Width(100f));
        bug.state = (Bug.BugState)EditorGUILayout.EnumPopup("", bug.state, GUILayout.Width(100));
        GUILayout.EndVertical();
        if(bug.state != Bug.BugState.Fixed && bug.state != Bug.BugState.Not_Fixing)
            GUI.enabled = false;
        if(bug.archived) {
            if(GUILayout.Button("Unarchive", GUILayout.Width(70))) {
                UnarchiveBug(bug);
            }
        } else {
            if(GUILayout.Button("Archive", GUILayout.Width(70))) {
                ArchiveBug(bug);
            }
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUI.backgroundColor = preColor;
        GUILayout.Space(5);
    }

    #endregion

    #region Misc Functions

    void RecalculateMaxPages() {
        if(ItemsToShowPerPage == 0)
            numberOfPages = 0;
        else
            numberOfPages = Mathf.CeilToInt((float)visibleList.Count / ItemsToShowPerPage);
        CurrentPage = 0;
    }

    void RecalculateVisibleList() {
        visibleList.Clear();

        List<Bug> targetList;
        if(ShowArchived) {
            targetList = bugList.archivedBugs;
        } else {
            targetList = bugList.bugs;
        }
        bool statePass = false;
        bool severityPass = false;

        for(int i = 0; i < targetList.Count; i++) {

            statePass = false;
            severityPass = false;

            if(ShowArchived != targetList[i].archived)
                continue;

            if(ShowArchived) {
                statePass = true;
            }

            if(ShowPending && targetList[i].state == Bug.BugState.Pending)
                statePass = true;
            if(ShowPossiblyFixed && targetList[i].state == Bug.BugState.Possibly_Fixed)
                statePass = true;
            if(ShowFixed && targetList[i].state == Bug.BugState.Fixed)
                statePass = true;
            if(ShowNotFixing && targetList[i].state == Bug.BugState.Not_Fixing)
                statePass = true;

            if(ShowLiterallyUnplayable && targetList[i].severity == Bug.BugSeverity.Literally_Unplayable)
                severityPass = true;
            if(ShowVisual && targetList[i].severity == Bug.BugSeverity.Visual)
                severityPass = true;
            if(ShowMinor && targetList[i].severity == Bug.BugSeverity.Minor)
                severityPass = true;
            if(ShowMajor && targetList[i].severity == Bug.BugSeverity.Major)
                severityPass = true;

            if(statePass && severityPass)
                visibleList.Add(targetList[i]);
        }
        RecalculateMaxPages();
    }

    void ArchiveBug(Bug bug) {
        bug.archived = true;
        bugList.bugs.Remove(bug);
        bugList.archivedBugs.Add(bug);
        bugList.archivedBugs.Sort(delegate(Bug x, Bug y) {
            return x.bugID.CompareTo(y.bugID);
        });
    }

    void UnarchiveBug(Bug bug) {
        bug.archived = false;
        bugList.archivedBugs.Remove(bug);
        bugList.bugs.Add(bug);
        bugList.bugs.Sort(delegate (Bug x, Bug y) {
            return x.bugID.CompareTo(y.bugID);
        });
    }

    #endregion
}
