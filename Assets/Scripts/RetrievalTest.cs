using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

// This class tests that the data recieved matches what was supposed to be sent

public class RetrievalTest
{
    private Queue<Dictionary<string, object>> dictQueue;

    // Creates an Queue containing dictionaries from a file that will
    // be used to test if data is being sent correctly
    public int ParseTestFile(string fileName)
    {
        try
        {
            string[] dictionaries = System.IO.File.ReadAllText(fileName).Split("|");
            dictQueue = new Queue<Dictionary<string, object>>();
            foreach (string dict in dictionaries)
            {
                dictQueue.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, object>>(dict));
            }
            return dictQueue.ToArray().Length;
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e.ToString());
            return 0;
        }
    }

    // Tests the most recent packet
    // Two Dictionaries are considered equal if both contain the same keys
    // with the same value. The keys do not have to be in the same relative
    // order as long as they do exist.
    public bool TestPacket(Dictionary<string, object> curr)
    {
        if (curr == null || dictQueue.Peek() == null)
        {
            return false;
        }
        Dictionary<string, object> next = dictQueue.Dequeue();
        if (next.Count != curr.Count)
        {
            return false;
        }
        foreach (KeyValuePair<string, object> entry in curr)
        {
            if (!next.ContainsKey(entry.Key))
            {
                return false;
            }
            else if (!entry.Value.Equals(next[entry.Key]))
            {
                return false;
            }
        }
        return true;
    }

}
