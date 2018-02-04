using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(RectTransform))]
public class UIGrid : MonoBehaviour
{
    [SerializeField]
    private bool _dotted;
    [SerializeField]
    private bool _goldenRatio;
    [SerializeField]
    private Color _gridColor;
    [SerializeField]
    private Color _intersectionColor;
    [SerializeField]
    private float _snapStrenght;
    [SerializeField]
    private int _gridAmount;


    private Vector3[] corners = new Vector3[4];
    private List<Line> _lines = new List<Line>();
    private List<Vector3> _intersections = new List<Vector3>();
    private GameObject _selectedGameObject;

    ///   Corners
    ///  1 ------ 2
    ///  -        -
    ///  -        -
    ///  -        -
    ///  0 ------ 3
    ///  

    private class Line
    {
        public Vector3 start;
        public Vector3 end;
        public bool horizontal;
    }


    private void OnDrawGizmos()
    {
        GetSelectedGameObject();
        if (_selectedGameObject != null && !_selectedGameObject.GetComponent<Canvas>() && _selectedGameObject.transform.parent == transform)
        {
            FindIntersection();
        }
        DrawGrid();
    }

    private void OnEnable()
    {
        GetCorners();
    }

    private void DrawGrid()
    {
        Handles.color = _gridColor;

        for (int i = 0; i < _lines.Count; i++)
        {
            if (_dotted) Handles.DrawDottedLine(_lines[i].start, _lines[i].end, 3f);
            else Handles.DrawLine(_lines[i].start, _lines[i].end);
        }

        Handles.color = _intersectionColor;
        for (int i = 0; i < _intersections.Count; i++)
        {

            Handles.CubeCap(0, _intersections[i], Quaternion.identity, 0.25f);
        }
    }
    private void OnDrawGizmosSelected()
    {
        _lines = new List<Line>();
        _intersections = new List<Vector3>();
        GetCorners();
        if(_goldenRatio) { GoldenRatio(); }
        else { Grid(); }
        DisplayIntersection();

    }

    void GetCorners()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.GetWorldCorners(corners);
    }

    void FindIntersection()
    {
        for (int i = 0; i < _intersections.Count; i++)
        {
            if (Vector3.Distance(_selectedGameObject.transform.position, _intersections[i]) < _snapStrenght)
                _selectedGameObject.transform.position = _intersections[i];
        }
    }

    void GetSelectedGameObject()
    {
        _selectedGameObject = Selection.activeGameObject;
    }

    void DisplayIntersection()
    {
        var horizontalLines = _lines.Where(x => x.horizontal).ToList();
        var verticalLines = _lines.Where(x => !x.horizontal).ToList();
        for (int i = 0; i < horizontalLines.Count(); i++)
        {
            for (int x = 0; x < verticalLines.Count(); x++)
            {
                Vector2 intersection = new Vector2();
                LineSegmentsIntersection(horizontalLines[i].start, horizontalLines[i].end, verticalLines[x].start, verticalLines[x].end, out intersection);
                Vector3 intersection3 = new Vector3(intersection.x, intersection.y, horizontalLines[i].start.z);
                _intersections.Add(intersection3);
            }
        }
    }

    void AddNewLine(float basenumber, float multiplier, bool horizontal, bool goldenratio = false, float ratioNumber = 0, bool inverted = false)
    {
        Line newLine = new Line();
        newLine.horizontal = horizontal;
        var c1 = horizontal ? corners[1] : corners[1];
        var c2 = horizontal ? corners[2] : corners[0];
        var c3 = horizontal ? corners[0] : corners[2];
        var c4 = horizontal ? corners[3] : corners[3];

        newLine.start = Vector3.Lerp(c1, c2, 1 / basenumber * multiplier);
        newLine.end = Vector3.Lerp(c3, c4, 1 / basenumber * multiplier);


        if (goldenratio)
        {
            newLine.start = Vector3.Lerp(c1, c2, ratioNumber);
            newLine.end = Vector3.Lerp(c3, c4, ratioNumber);
        }

        if (goldenratio && inverted)
        {
            newLine.start = Vector3.Lerp(c2, c1, ratioNumber);
            newLine.end = Vector3.Lerp(c4, c3, ratioNumber);
        }
        _lines.Add(newLine);
    }

    void Grid()
    {
        for (int i = 1; i < _gridAmount; i++)
        {
            AddNewLine(_gridAmount, i, true);
            AddNewLine(_gridAmount, i, false);
        }
    }

    void GoldenRatio()
    {
        float fulllenght = 1;
        for (int i = 1; i < _gridAmount; i++)
        {
            //Vertical +
            AddNewLine(0f, 0f, true, true, 0.6180339887f * fulllenght);
            fulllenght = fulllenght * 0.6180339887f;
        }
        fulllenght = 1;
        for (int i = 1; i < _gridAmount; i++)
        {
            //Vertical -
            AddNewLine(0f, 0f, true, true, 0.6180339887f * fulllenght, true);
            fulllenght = fulllenght * 0.6180339887f;
        }

        fulllenght = 1;
        for (int i = 1; i < _gridAmount; i++)
        {
            //Horizontal +
            AddNewLine(0f, 0f, false, true, 0.6180339887f * fulllenght);
            fulllenght = fulllenght * 0.6180339887f;
        }

        fulllenght = 1;
        for (int i = 1; i < _gridAmount; i++)
        {
            //Horizontal -
            AddNewLine(0f, 0f, false, true, 0.6180339887f * fulllenght, true);
            fulllenght = fulllenght * 0.6180339887f;
        }
    }


    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }
}