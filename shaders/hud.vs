#version 330 core
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aTex;

uniform mat4 u_Model;
uniform mat4 u_Projection;
uniform vec4 u_TexCoords;
uniform int u_Rotation;

out vec2 TexCoord;

void main()
{
    gl_Position = u_Projection * u_Model * vec4(aPos, 0.0, 1.0);
   // TexCoord = aTex;

    vec2 baseUV = aPos;
    //baseUV.y = 1 - baseUV.y;
    vec2 rotatedUV;

    if (u_Rotation == 0) {
        rotatedUV = baseUV;
    } else if (u_Rotation == 3) {
        rotatedUV = vec2(baseUV.y, 1.0 - baseUV.x);
    } else if (u_Rotation == 2) {
        rotatedUV = vec2(1.0 - baseUV.x, 1.0 - baseUV.y);
    } else if (u_Rotation == 1) {
        rotatedUV = vec2(1.0 - baseUV.y, baseUV.x);
    } else {
       rotatedUV = baseUV;
    }

    TexCoord = mix(u_TexCoords.xy, u_TexCoords.zw, rotatedUV);
}
