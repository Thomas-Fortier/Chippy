﻿using Chippy.Program;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Text;

namespace Chippy.Programe
{
  internal class Program
  {
    private static void Main()
    {
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Chip8 emulator Logo [Garstyciuks].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\IBM Logo.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Chip8 Picture.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Particle Demo [zeroZshadow, 2008].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Random Number Test [Matthew Mikolay, 2010].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Zero Demo [zeroZshadow, 2007].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Clock Program [Bill Fisher, 1981].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Brix [Andreas Gustafsson, 1990].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Tetris [Fran Dachille, 1991].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Space Invaders [David Winter].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Trip8 Demo (2008) [Revival Studios].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Maze [David Winter, 199x].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Stars [Sergey Naydenov, 2010].ch8");
      byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Breakout [Carmelo Cortez, 1979].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Pong (1 player).ch8");
      StringBuilder sb = new StringBuilder();
      List<byte> bytes = new List<byte>();

      foreach (byte b in fileBytes)
      {
        bytes.Add(b);
        sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
      }

      var emulator = CreateEmulator(bytes.ToArray());
      emulator.Start();
    }

    private static Emulator CreateEmulator(byte[] data)
    { 
      var window = CreateWindow();
      var memory = new Memory(new byte[4096], new Stack<ushort>());
      var processor = new Processor(memory, window, new byte[16]);
      var emulator = new Emulator(processor, window, data);

      return emulator;
    }

    private static Window CreateWindow()
    {
      var gameSettings = new GameWindowSettings
      {
        UpdateFrequency = 600
      };

      var nativeSettings = new NativeWindowSettings
      {
        Size = new Vector2i(1024, 512),
        Profile = ContextProfile.Compatability,
        Title = "Chippy"
      };

      const int NATIVE_WIDTH = 64;
      const int NATIVE_HEIGHT = 32;

      return new Window(gameSettings, nativeSettings, NATIVE_WIDTH, NATIVE_HEIGHT);
    }
  }
}