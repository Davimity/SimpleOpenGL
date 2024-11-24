using Silk.NET.OpenGL;

namespace SimpleOpenGL.Structs;

public struct VertexAttribute {
    public uint Index;
    public int Size;
    public VertexAttribPointerType Type;
    public bool Normalized;
    public uint Stride;
}