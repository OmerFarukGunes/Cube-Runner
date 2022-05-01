
using UnityEngine;
using DG.Tweening;
public class Cube : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] float Intensity = 1f;
    [SerializeField] float Mass = 1f;
    [SerializeField] float stiffness = 1f;
    [SerializeField] float damping = 0.75f;

    private Mesh OriginalMesh, MeshClone;
    private MeshRenderer renderer;
    private Vertex[] jv;
    private Vector3[] vertexArray;
    private Vector3 target;
    float intensity;
    void Start()
    {
        OriginalMesh = GetComponent<MeshFilter>().sharedMesh;
        MeshClone = Instantiate(OriginalMesh);
        GetComponent<MeshFilter>().sharedMesh = MeshClone;
        renderer = GetComponent<MeshRenderer>();
        jv = new Vertex[MeshClone.vertices.Length];
        for (int i = 0; i < MeshClone.vertices.Length; i++)
        {
            jv[i] = new Vertex(i, transform.TransformPoint(MeshClone.vertices[i]));
        }

    }
    void FixedUpdate()
    {
        vertexArray = OriginalMesh.vertices;
        for (int i = 0; i < jv.Length; i++)
        {
            target = transform.TransformPoint(vertexArray[jv[i].ID]);
            intensity = (1 - (renderer.bounds.max.y - target.y) / renderer.bounds.size.y) * Intensity;
            jv[i].Shake(target, Mass, stiffness, damping);
            target = transform.InverseTransformPoint(jv[i].Position);
            vertexArray[jv[0].ID] = Vector3.Lerp(vertexArray[jv[i].ID], target, intensity);
        }
        MeshClone.vertices = vertexArray;
    }

    public class Vertex
    {
        public int ID;
        public Vector3 Position;
        public Vector3 velocity, Force;

        public Vertex(int _id, Vector3 _pos)
        {
            ID = _id;
            Position = _pos;
        }

        public void Shake(Vector3 target, float m, float s, float d)
        {
            Force = (target - Position) * s;
            velocity = (velocity + Force / m) * d;
            Position += velocity;
            if ((velocity + Force + Force / m).magnitude < 0.001f)
                Position = target;
        }
    }
}

