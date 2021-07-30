#version 300 es
precision mediump float;       	// Set the default precision to medium

uniform sampler2D u_Texture;   	// texture
in vec2 v_UV;			// Texture UV coordinate
in vec4 v_diffuseColour;	// Diffuse colour
in vec4 v_fogColour;		// Fog colour
out vec4 FragColor;

void main()
{
		vec4 col = texture2D(u_Texture, v_UV) * v_diffuseColour + v_fogColour;
		if (col.a > 0.01) FragColor = col; else discard;
}
