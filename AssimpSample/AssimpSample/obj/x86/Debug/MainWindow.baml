   M S B A M L   `   `   `  ����R  NSharpGL.WPF, Version=2.3.0.1, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF  5   	    AssimpSample   AssimpSample.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BsharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF     5   	   -   5          � Title$  SharpGL WPF Application��5   	   $	��500��6)   $	��950��66   .��5      � .+�5	   
      SharpGL.WPF.OpenGLControl  -   5        $��openGLControl��5
         DrawFPS .    RenderContextType$	 FBO=�5         	FrameRate$ 10Q�64   $��	200,0,0,0q�6C   �� 5   
   $��Goal Height:��6   $
��Left=�5      $��5,10,0,0q�5      $	��Top=�5      $��80��5      �� 5   
   -   5        $��goalHeightCombo��5      $
��Left=�5      $��	90,10,0,0q�5      $	��Top=�5      $	��100��5      ��5      ��  �� Path$ 
GoalHeight� �� SelectedItem 5      ��  �� Mode$ TwoWay=�$ 
GoalHeight�5      �� 5   
   $��
Ball Size:��6   $
��Left=�5      $��
5,50,0,100q�5      $	��Top=�5      $��80��5      �� 5    
   -   5        $��ballSizeCombo��5       $
��Left=�5!      $��90,50,0,100q�5"      $	��Top=�5#      $	��100��5$      ��5%      �� $ BallSize� 5&      �� $ TwoWay=�$ BallSize�5'      �� 5)   
   $��Speed:��6   $
��Left=�5*      $��
5,90,0,100q�5+      $	��Top=�5,      $��80��5-      �� 5.   
   -   5        $��
speedCombo��5.      $
��Left=�5/      $��90,90,0,100q�50      $	��Top=�51      $	��100��52      ��53      �� $ SpeedOfRotation� 54      �� $ TwoWay=�$ SpeedOfRotation�55      �� 57   
   -   5        $��
kickButton��57      $
��Left=�6#   $��50,150,0,100q�58      $	��Top=�59      $	��100��5;      .��6   Kick6%   �� 5<   
   -   5        $��startPositionButton��5<      $
��Left=�6,   $��50,200,0,100q�5=      $	��Top=�5>      $	��100��5@      .��6   	Restart6(   5A      5B      