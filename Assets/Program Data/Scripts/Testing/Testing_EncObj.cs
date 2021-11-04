using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_EncObj
{
    public string m_str;
    public int m_int;
    
    public Testing_EncObj(){}

    public Testing_EncObj(string s, int i)
    {
        m_str = s;
        m_int = i;
    }

    public override string ToString()
    {
        return "Test object: " + m_str + " | " + m_int;
    }
}
