using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
public class UILine : MaskableGraphic
{
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float thickness = 1f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 lineDirection = endPoint - startPoint;
        Vector2 perpendicular = new Vector2(
            -lineDirection.y,
            lineDirection.x).normalized;

        Vector2 offset1 = perpendicular * thickness * 0.5f;
        Vector2 offset2 = -perpendicular * thickness * 0.5f;

        UIVertex vertex1 = UIVertex.simpleVert;
        vertex1.color = color;
        vertex1.position = startPoint + offset1;

        UIVertex vertex2 = UIVertex.simpleVert;
        vertex2.color = color;
        vertex2.position = startPoint + offset2;

        UIVertex vertex3 = UIVertex.simpleVert;
        vertex3.color = color;
        vertex3.position = endPoint + offset1;

        UIVertex vertex4 = UIVertex.simpleVert;
        vertex4.color = color;
        vertex4.position = endPoint + offset2;

        vh.AddVert(vertex1);
        vh.AddVert(vertex2);
        vh.AddVert(vertex3);
        vh.AddVert(vertex4);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 1, 3);
    }
}