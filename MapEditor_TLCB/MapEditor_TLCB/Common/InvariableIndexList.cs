﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MapEditor_TLCB.Common
{
    [Serializable()]
    class InvariableIndexList<T> : ISerializable
    {
        public InvariableIndexList()
        {
            m_list=new List<T>();
	        m_freeIndices=new Stack<int>();
        }

        public InvariableIndexList(SerializationInfo info, StreamingContext ctxt)
		{
            m_list = (List<T>)info.GetValue("List", typeof(List<T>));
            m_freeIndices = (Stack<int>)info.GetValue("FreeIndices", typeof(Stack<int>));
		}

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("List", m_list);
            info.AddValue("FreeIndices", m_freeIndices);
        }

        public int add(T p_valueRef)
        {
            int index = -1;
	        if (m_freeIndices.Count()>0)
	        {
		        index = m_freeIndices.First();
		        m_list[index] = p_valueRef;
		        m_freeIndices.Pop();
	        }
	        else
	        {
		        m_list.Add(p_valueRef);
		        index = m_list.Count()-1;
	        }
	        return index;
        }

	    public bool removeAt(int p_index)
        {
	        if (p_index<m_list.Count())
	        {
		        m_list[p_index] = default(T);
		        m_freeIndices.Push(p_index);
		        return true;
	        }
	        return false;
        }

	    public T at(int p_index)
        {
            return m_list[p_index];
        }
	    public T this[int p_index]
        {
            get
            {
                return m_list[p_index];
            }
        }

	    public int getSize()
        {
            return m_list.Count();
        }

	    public void	clear()
        {
            m_list.Clear();
            while (m_freeIndices.Count()>0)
                m_freeIndices.Pop();
        }

	    private List<T> m_list;
	    private Stack<int> m_freeIndices;
    }
}
