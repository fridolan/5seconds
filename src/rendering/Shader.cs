using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Reflection;
using System.IO;

namespace fiveSeconds
{
	public class Shader
	{
		public int Handle { get; private set; }

		public Shader(string vertexSourcePath, string fragmentSourcePath)
		{

			string vertexSource = File.ReadAllText("shaders/" + vertexSourcePath);
			string fragmentSource = File.ReadAllText("shaders/" + fragmentSourcePath);

			int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, vertexSource);
			GL.CompileShader(vertexShader);
			CheckShaderCompile(vertexShader, "VERTEX");

			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, fragmentSource);
			GL.CompileShader(fragmentShader);
			CheckShaderCompile(fragmentShader, "FRAGMENT");

			Handle = GL.CreateProgram();
			GL.AttachShader(Handle, vertexShader);
			GL.AttachShader(Handle, fragmentShader);
			GL.LinkProgram(Handle);
			CheckProgramLink(Handle);

			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);
		}

		public void Use()
		{
			GL.UseProgram(Handle);
		}

		public void SetMatrix4(string name, Matrix4 matrix)
		{
			int location = GL.GetUniformLocation(Handle, name);
			if (location == -1)
				Console.WriteLine($"Warning: uniform '{name}' not found.");
			else
				GL.UniformMatrix4(location, false, ref matrix);
		}

		public void SetInt(string name, int value)
		{
			int location = GL.GetUniformLocation(Handle, name);
			if (location == -1)
				Console.WriteLine($"Warning: uniform '{name}' not found.");
			else
				GL.Uniform1(location, value);
		}

		public void SetFloat(string name, float value)
		{
			int location = GL.GetUniformLocation(Handle, name);
			if (location == -1)
				Console.WriteLine($"Warning: uniform '{name}' not found.");
			else
				GL.Uniform1(location, value);
		}

		public void SetVector2(string name, Vector2 vec)
		{
			int location = GL.GetUniformLocation(Handle, name);
			if (location == -1)
				Console.WriteLine($"Warning: uniform '{name}' not found.");
			else
				GL.Uniform2(location, vec);
		}

		public Vector2 GetVector2(string name)
		{
			int location = GL.GetUniformLocation(Handle, name);
			if (location == -1)
			{
				Console.WriteLine($"Warning: uniform '{name}' not found.");
				throw new Exception();
			}
			else
			{
				float[] values = new float[2];
				GL.GetUniform(Handle, location, values);
				return (values[0], values[1]);
			}
		}

		public float GetFloat(string name)
		{
			int location = GL.GetUniformLocation(Handle, name);
			if (location == -1)
			{
				Console.WriteLine($"Warning: uniform '{name}' not found.");
				throw new Exception();
			}
			else
			{
				float[] values = new float[1];
				GL.GetUniform(Handle, location, values);
				return values[0];
			}
		}

		public float GetInt(string name)
		{
			int location = GL.GetUniformLocation(Handle, name);
			if (location == -1)
			{
				Console.WriteLine($"Warning: uniform '{name}' not found.");
				throw new Exception();
			}
			else
			{
				int[] values = new int[1];
				GL.GetUniform(Handle, location, values);
				return values[0];
			}
		}


		public void SetVector3(string name, Vector3 vec)
		{
			int location = GL.GetUniformLocation(Handle, name);
			if (location == -1)
				Console.WriteLine($"Warning: uniform '{name}' not found.");
			else
				GL.Uniform3(location, vec);
		}

		private void CheckShaderCompile(int shader, string type)
		{
			GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
			if (success == 0)
			{
				string log = GL.GetShaderInfoLog(shader);
				throw new Exception($"{type} SHADER compilation failed:\n{log}");
			}
		}

		private void CheckProgramLink(int program)
		{
			GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
			if (success == 0)
			{
				string log = GL.GetProgramInfoLog(program);
				throw new Exception($"SHADER PROGRAM linking failed:\n{log}");
			}
		}
	}
}
