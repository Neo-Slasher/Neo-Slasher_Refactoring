using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StoryList {
    public List<Story> stories;
}

[Serializable]
public class Story {
    public List<string> story;
}
