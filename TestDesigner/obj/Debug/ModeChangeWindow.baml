   M S B A M L   `   `   `  ����G  CTestDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TestDesignerTestDesigner  5   	   #    TestDesigner.ModeChangeWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3testDesignerclr-namespace:TestDesigner     5   	     � Title$  ModeChangeWindow��5   	   $��80��6"   $��80��6.    � WindowStyle$
 None=�69   $��1q�6L   
����  � � 	Resources 5         A1Style �� 
TargetType �� Triggers �� Binding �� RelativeSource �� Mode �� AncestorType#   TestDesigner.ModeChangeWindow	 �� Path
 �� Value �� Property �� Value  A2Style��%�  &	        &	 �     �� 5   
   " 	�6     5      �� 5	       5
      ��  �� $ FindAncestor=�"  $
	 Mode�
 EDIT5      .a�5      ��  �@d�	IsChecked$
 trueY�62   5      5      �� 5   
   " 	�6     5      �� 5       5      ��  �� $ FindAncestor=�"  $
	 Mode�
 VIEW5      .a�5      ��  �@d�	IsChecked$
 trueY�62   5      5      5      .��5      �� $��模式切换��6   $	��Top=�6   $��76��65   $
��Left=�6A   $��76��6\   .��5   
   � $	��Top=�6   $
��Left=�6'   .+�5      	� -   5        $��a1��5   L   #��[  6    d� 	IsChecked$
 TrueY�6;   $��设计��6V   $��Center=�6c   $��	6,9,-6,-9q�6�   	� 5      -   5        $��a2��5   ;   #��[ 6   $��预览��6E   $��Center=�6R   $��6,30,-6,-30q�6o   5      5      5!      