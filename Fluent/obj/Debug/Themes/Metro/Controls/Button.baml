   M S B A M L   `   `   `  ����M  IFluent, Version=2.1.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546fclr-namespace:FluentFluent  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089#    Fluent.ObjectToImageConverter   StringToImageConvert  RibbonButtonControlTemplate  �� 
TargetType   Fluent.Button ��Name  TransparentBrush ��Name �� Orientation ��Name J� SnapsToDevicePixels �� RelativeSource �� 	Converter   Fluent.TwoLineLabel  Name	   Header
   Text  TwoLineLabelStyle �� Triggers P� Property P� Value �� 
TargetName �� Property �� Value ! ButtonPressedOuterBorderBrush % !ButtonPressedOuterBackgroundBrush   Fluent.GrayscaleEffect ]� 
Conditions �� Property �� Value # ButtonHoverOuterBackgroundBrush  ButtonHoverOuterBorderBrush  RibbonButtonStyle �� 
TargetType 	 DefaultFontSize 
 DefaultFontFamily  DefaultFontBrush  ControlStyleEmptyFocus �� Triggers   Fluent.RibbonControl �� BasedOn ( $IronicallyNamedChromelessButtonStyle  TransparentWhiteBrush �Name J� Opacity�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Fluentclr-namespace:Fluent  5      8d2http://schemas.microsoft.com/expression/blend/2008  6'   Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  6d   %�  &	        &	      7   7   7   &	 m    7  '  \    7  &	 ~       5      6"   �� 5      "   68   .R�5      �� $ border��6   #���  6   #���  6M   $
��Auto��6~   $
��Left=�6�   $��Stretch=�6�   ._�5      � $
��Auto��6   .+�5      �� $ 
stackPanel��6   $
��Auto��6+   $ Vertical=�68   .+�5	      �� $ 	iconImage��6   $��Center=�62   $��32��6O   $��Center=�6[   $��32��6�   $��3,2,3,1q�6   . ��6v   �� *	LargeIcon+ �� *TemplatedParent+8   6    5
      $ controlLabel��6"   #
 z	 68   #���  6X   $��Stretch=�6�   $��Center=�6�   $��2,-1,2,0q�6�   5      5      5       5      P� 5       �@ Size$ Small=�6    . �5   
   �� $ 
stackPanel��6(    �@��Orientation$ 
Horizontal=�6@   �� 5   
   $ controlLabel��6'    � y�$ 	Collapsed=�6A   �� 5   
   $ controlLabel��6(    �@ HasTwoLines .  6B   �� 5   
   $ 	iconImage��6"    � ��$ 16��69   �� 5   
   $ 	iconImage��6#    � ��$ 16��6:   �� 5   
   $ 	iconImage��6#    � ��$ 1,0,2,0q�6:   �� 5   
   $ 	iconImage��6$    6;   �� *Icon+ �� *TemplatedParent+8   � ���� 5   
   $ controlLabel��6#    � ��$ 2,-1,2,1q�6=   5   	   P� 5       �@ Size$ Middle=�6    . �5   
   �� $ 
stackPanel��6(    �@��Orientation$ 
Horizontal=�6@   �� 5   
   $ 
stackPanel��6"    � ��$
 Auto��6:   �� 5   
   $ 	iconImage��6.    � ��$ Center=�6E   �� 5   
   $ controlLabel��6(    �@ HasTwoLines .  6B   �� 5   
   $ controlLabel��6#    � ��$ 2,-1,2,1q�6=   �� 5   
   $ border��6.    � ��$ Stretch=�6B   �� 5    
   $ 	iconImage��6"    � ��$ 16��69   �� 5!   
   $ 	iconImage��6#    � ��$ 16��6:   �� 5"   
   $ 	iconImage��6#    � ��$ 1,0,2,0q�6:   �� 5#   
   $ 	iconImage��6$    6;   �� *Icon+ �� *TemplatedParent+8   � ���� 5$   
   $ controlLabel��6.    � ��$ Center=�6H   5%   	   P� 5&       � �� . . �5'   
   �� $ border��6(   # �  6<    � ���� 5(   
   $ border��6'   # �  6;    � ��5)   	   P� 5*       � {� .  . �5+   
   �� $ 	iconImage��6$    �@J�Opacity$	 0.5Q�6;   �� 5,   
   $ controlLabel��6$    �@J�Opacity$	 0.5Q�6>   �� 5-   
   $ 	iconImage��6#    �@J�Effect 5.        5/      51      52   	   ]� 53       54   
   �� 55       �@J�IsMouseOver . 6-   �� 56       � �� .  6+   �� 57       � {� . 6+   ./�59   
   �� $ border��6'   # �  6;    � ���� 5:   
   $ border��6(   # �  6<    � ��5;   	   ]� 5<       5=   
   �� 5>       �@J�IsKeyboardFocusWithin . 67   �� 5?       � �� .  6+   ./�5A   
   �� $ border��6'   # �  6;    � ���� 5B   
   $ border��6(   # �  6<    � ��5C   	   5E      �� 5F      "  6$   .�5G      ��  � ��8   6!   �� 5H       � ��$ 66��6   �� 5I       � ��$
 Auto��6   �� 5J       � ��# � 	 6!   �� 5K       � ��# � 
 6#   �� 5L       � ��# �  6#   �� 5M       �@�FocusVisualStyle# �  6)   �� 5N       �@J�SnapsToDevicePixels .  6,    5O      P� 5P       �@ Size$ Large=�65   . �5Q   
   ��  � ��$ 66��6#   5R   	   P� 5S       �@ Size$ Middle=�65   . �5T   
   ��  � ��$ 22��6#   �� 5U   
    � ��$
 Left=�60   5V   	   P� 5W       �@ Size$ Small=�65   . �5X   
   ��  � ��$ 22��6#   �� 5Y   
    � ��$ 22��6"   �� 5Z   
    � ��$
 Left=�60   5[   	   5]      �� 5_      "  6
   8   6.   �� 5a      " ��67   .�5b   
   ��  � ��# �  6'   �� 5c   
    � ��# � ��6'   �� 5d   
    � ��$ Center=�67   �� 5e   
    � ��$ Center=�65   �� 5f   
    � ��$ 1q�6$   �� 5g   
    � �� 5h      �� 5i      "  ��6"   .R�5j      � $
 grid��6   #��z��6)   .+�5k      �� $ contentPresenter��6+   #��z��5l   +   #��z��5m   +   #��z��5n   +   # z 5o   +   ��. $
 0.75Q�5q   +   5r       5s      P� 5t       �@J�IsMouseOver . . �5u      �� $ contentPresenter��6%    �@J�Opacity$ 1Q�6V   �� 5v      $
 grid��6%   # �  6M    � ��5w      P� 5x       �@J�IsMouseOver .  . �5y      �� $ contentPresenter��6%    �@J�Opacity$ .5Q�6V   5z      P� 5{       � �� . . �5|      �� $
 grid��6%   # �  6M    � ��5}      P� 5~       � {� .  . �5      ��  � �� �#ADADAD6;   5�      5�      5�      5�      5�      