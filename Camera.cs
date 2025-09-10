using Godot;
using System;

public partial class Camera : Camera2D
{
	// 相机移动速度
	private float velocity = 10.0f;
	
	// 相机移动边界（动态计算）
	private float leftBound, rightBound, topBound, bottomBound;

	// 地图引用，用于获取地图尺寸和坐标转换
	HexTileMap map;
	
	// 缩放相关变量
	private float zoom_speed = 0.07f;  // 缩放速度
	private bool mouseWheelScrollingUp = false;    // 鼠标滚轮向上滚动标志
	private bool mouseWheelScrollingDown = false;  // 鼠标滚轮向下滚动标志

	public override void _Ready()
	{
		// 获取地图节点的引用
		map = GetNode<HexTileMap>("../HexTileMap");
		
		// 计算相机移动边界
		// 左边界：地图左上角 + 100像素边距
		leftBound = ToGlobal(map.MapToLocal(new Vector2I(0, 0))).X + 100;
		// 右边界：地图右下角 - 100像素边距
		rightBound = ToGlobal(map.MapToLocal(new Vector2I(map.width, map.height))).X - 100;
		// 上边界：地图左上角 + 50像素边距
		topBound = ToGlobal(map.MapToLocal(new Vector2I(0, 0))).Y + 50;
		// 下边界：地图右下角 - 50像素边距
		bottomBound = ToGlobal(map.MapToLocal(new Vector2I(map.width, map.height))).Y - 50;
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
