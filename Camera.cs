using Godot;
using System;

public partial class Camera : Camera2D
{
	// 相机移动速度
	private float velocity = 5.0f;
	
	// 相机移动边界
	private float leftBound = 0;
	private float rightBound = 1000;
	private float topBound = 0;
	private float bottomBound = 1000;

	// 缩放相关变量
	private float zoom_speed = 0.1f;  // 缩放速度
	private bool mouseWheelScrollingUp = false;    // 鼠标滚轮向上滚动标志
	private bool mouseWheelScrollingDown = false;  // 鼠标滚轮向下滚动标志

	public override void _Process(double delta)
	{
		base._Process(delta);
		// 处理向上移动的即时响应
		if(Input.IsActionJustPressed("ui_up")){
			Position += new Vector2(0, -1);
		}
	}

	public override void _PhysicsProcess(double delta){
		// 处理向右移动
		// 检查是否按下右键且未超出右边界
		if (Input.IsActionPressed("map_right") && this.Position.X < rightBound)
		{
			// 向右移动相机
			this.Position += new Vector2(velocity, 0);
		}

		// 处理向左移动
		// 检查是否按下左键且未超出左边界
		if (Input.IsActionPressed("map_left") && this.Position.X > leftBound)
		{
			// 向左移动相机
			this.Position += new Vector2(-velocity, 0);
		}

		// 处理向上移动
		// 检查是否按下上键且未超出上边界
		if (Input.IsActionPressed("map_up") && this.Position.Y > topBound)
		{
			// 向上移动相机
			this.Position += new Vector2(0, -velocity);
		}

		// 处理向下移动
		// 检查是否按下下键且未超出下边界
		if (Input.IsActionPressed("map_down") && this.Position.Y < bottomBound)
		{
			// 向下移动相机
			this.Position += new Vector2(0, velocity);
		}

		// 处理放大操作
		// 检查是否按下放大键或鼠标滚轮向上滚动
		if (Input.IsActionPressed("map_zoom_in") || mouseWheelScrollingUp)
		{
			// 检查是否未超过最大缩放限制（3倍）
			if (this.Zoom < new Vector2(3f, 3f))
				// 增加缩放值
				this.Zoom += new Vector2(zoom_speed, zoom_speed);
		}

		// 处理缩小操作
		// 检查是否按下缩小键或鼠标滚轮向下滚动
		if (Input.IsActionPressed("map_zoom_out") || mouseWheelScrollingDown)
		{
			// 检查是否未超过最小缩放限制（0.1倍）
			if (this.Zoom > new Vector2(0.1f, 0.1f))
				// 减小缩放值
				this.Zoom -= new Vector2(zoom_speed, zoom_speed);
		}
	}
}
