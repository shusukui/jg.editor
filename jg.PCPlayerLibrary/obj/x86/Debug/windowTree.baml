   M S B A M L   `   `   `  ����L  Hjg.Editor.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullP:clr-namespace:jg.Editor.Library;assembly=jg.Editor.Libraryjg.Editor.Library  5   	    jg.PCPlayerLibrary#   jg.PCPlayerLibrary.windowTree   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmy2:clr-namespace:jg.Editor.Library;assembly=jg.Editor.Library     5   	     � Title$  
windowTree��5   	    � WindowStyle$
 None=�62    � AllowsTransparency . $	��703��5   #   $	��200��60    � ShowInTaskbar .  ��5      Z�  � 	Resources 5      ,   &jg.Editor.Library.DataConverterVisible   dc �� 
TargetType �� Property �� Value �� DataType(   "jg.Editor.Library.TreeViewItemInfo	 �� ItemsSource
 �� Stretch �� 	Converter �� UpdateSourceTrigger��%[  &	        'Q�      (�� �     5   #   *, +)7     5	   
   6#   �� 5
   
   " Q�6   .�5      ��  �@Q�
IsExpanded . 6+   �� 5       �@Q�
IsSelected 6+   �� *
IsSelected+5      �� 5   
   "  6#   	 5   #   �� *
Children+.R�5      � .+�5      F� +����        �    @B � �  � �        	$

 Fill=�67   
�������$��2��5      
���   �$��10��5      $��10��6"   $
��Left=�5   X   y�6   �� *
IsSelected+8   6X   �� 5      $��10,0,0,0q�6   
���������5      �� *Title+$ PropertyChanged=�5      5      5      .��5      R� -   5         R�Name$ treeView��5#      $��0q�5   :   $��18(�6N   ���	#330000005#      5%      