#version 330 core

layout(location = 0) in vec2 aPosition;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec4 uTexCoords;
uniform int uRotation;

out vec2 TexCoord;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 0.0, 1.0);

    vec2 baseUV = aPosition;
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
