using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MdTranslatorLibrary
{

    public class BranchRef
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }
        public string sha { get; set; }
    }


    public class Branch
    {
        public string name { get; set; }
        public Commit commit { get; set; }
    }

    public class Commit
    {
        public string sha { get; set; }

    }


    public class Search
    {
        public string sha { get; set; }
        public string url { get; set; }
        public Tree[] tree { get; set; }
        public bool truncated { get; set; }
    }

    public class Tree
    {
        public string path { get; set; }
        public string mode { get; set; }
        public string type { get; set; }
        public string sha { get; set; }
        public int size { get; set; }
        public string url { get; set; }
    }


    public class Content
    {
        public string name { get; set; }
        public string path { get; set; }
        public string sha { get; set; }
        public int size { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string git_url { get; set; }
        public string download_url { get; set; }
        public string type { get; set; }
        public string content { get; set; }
        public string encoding { get; set; }
    }


    public class Class1
    {
        public Translation[] translations { get; set; }
    }

    public class Translation
    {
        public string text { get; set; }
        public string to { get; set; }
    }

    public class NewFile
    {
        public string message { get; set; }
        public Commiter commiter { get; set; }
        public string content { get; set; }
        public string branch { get; set; }
        public string sha { get; set; }
    }

    public class Commiter
    {
        public string name { get; set; }
        public string email { get; set; }
    }
}
