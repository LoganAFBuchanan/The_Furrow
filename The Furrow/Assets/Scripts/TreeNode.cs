using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from Aaron Gage and Steve Morgan's answer to this stack exchange question
// https://stackoverflow.com/questions/66893/tree-data-structure-in-c-sharp/2012855#2012855

public delegate void TreeVisitor<T>(T nodeData);

public class TreeNode<T>
{
    private T data;
    private LinkedList<TreeNode<T>> children;

    public TreeNode(T data)
    {
         this.data = data;
        children = new LinkedList<TreeNode<T>>();
    }

    public void AddChild(T data)
    {
        children.AddFirst(new TreeNode<T>(data));
    }

    public TreeNode<T> GetChild(int i)
    {
        foreach (TreeNode<T> n in children)
            if (--i == 0)
                return n;
        return null;
    }

    public void Traverse(TreeNode<T> node, TreeVisitor<T> visitor)
    {
        visitor(node.data);
        foreach (TreeNode<T> kid in node.children)
            Traverse(kid, visitor);
    }
}