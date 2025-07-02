using System;

// Token: 0x0200002A RID: 42
public enum BodypartType
{
	// Token: 0x04000252 RID: 594
	Hip,
	// Token: 0x04000253 RID: 595
	Mid,
	// Token: 0x04000254 RID: 596
	Torso,
	// Token: 0x04000255 RID: 597
	Neck,
	// Token: 0x04000256 RID: 598
	Head,
	// Token: 0x04000257 RID: 599
	Arm_L,
	// Token: 0x04000258 RID: 600
	Elbow_L,
	// Token: 0x04000259 RID: 601
	Hand_L,
	// Token: 0x0400025A RID: 602
	Arm_R,
	// Token: 0x0400025B RID: 603
	Elbow_R,
	// Token: 0x0400025C RID: 604
	Hand_R,
	// Token: 0x0400025D RID: 605
	Leg_L,
	// Token: 0x0400025E RID: 606
	Knee_L,
	// Token: 0x0400025F RID: 607
	Foot_L,
	// Token: 0x04000260 RID: 608
	Leg_R,
	// Token: 0x04000261 RID: 609
	Knee_R,
	// Token: 0x04000262 RID: 610
	Foot_R,
	// Token: 0x04000263 RID: 611
	Item,
	// Token: 0x04000264 RID: 612
	Mouth,
	// Token: 0x04000265 RID: 613
	Jaw_U,
	// Token: 0x04000266 RID: 614
	Jaw_D,
	// Token: 0x04000267 RID: 615
	Jaw_L,
	// Token: 0x04000268 RID: 616
	Jaw_R,
	// Token: 0x04000269 RID: 617
	Hip_L,
	// Token: 0x0400026A RID: 618
	Hip_R,
	// Token: 0x0400026B RID: 619
	Shoulder_L,
	// Token: 0x0400026C RID: 620
	Shoulder_R,
	// Token: 0x0400026D RID: 621
	Toe_L,
	// Token: 0x0400026E RID: 622
	Toe_R,
	// Token: 0x0400026F RID: 623
	Finger_L,
	// Token: 0x04000270 RID: 624
	Finger_R,
	// Token: 0x04000271 RID: 625
	Unnasigned_1,
	// Token: 0x04000272 RID: 626
	Unnasigned_2,
	// Token: 0x04000273 RID: 627
	Unnasigned_3,
	// Token: 0x04000274 RID: 628
	Unnasigned_4,
	// Token: 0x04000275 RID: 629
	Unnasigned_5,
	// Token: 0x04000276 RID: 630
	Unnasigned_6,
	// Token: 0x04000277 RID: 631
	Unnasigned_7,
	// Token: 0x04000278 RID: 632
	Unnasigned_8,
	// Token: 0x04000279 RID: 633
	Tail_1,
	// Token: 0x0400027A RID: 634
	Tail_2,
	// Token: 0x0400027B RID: 635
	Tail_3,
	// Token: 0x0400027C RID: 636
	Tail_4,
	// Token: 0x0400027D RID: 637
	Tail_5,
	// Token: 0x0400027E RID: 638
	Tail_6,
	// Token: 0x0400027F RID: 639
	Tail_7,
	// Token: 0x04000280 RID: 640
	Tail_8,
	// Token: 0x04000281 RID: 641
	Extra_1,
	// Token: 0x04000282 RID: 642
	Extra_2,
	// Token: 0x04000283 RID: 643
	Extra_3,
	// Token: 0x04000284 RID: 644
	Extra_4,
	// Token: 0x04000285 RID: 645
	Extra_5,
	// Token: 0x04000286 RID: 646
	Extra_6,
	// Token: 0x04000287 RID: 647
	Extra_7,
	// Token: 0x04000288 RID: 648
	Extra_8,
	// Token: 0x04000289 RID: 649
	Extra_9,
	// Token: 0x0400028A RID: 650
	Extra_10,
	// Token: 0x0400028B RID: 651
	Extra_11,
	// Token: 0x0400028C RID: 652
	Extra_12,
	// Token: 0x0400028D RID: 653
	Leg_1_L,
	// Token: 0x0400028E RID: 654
	Knee_1_L,
	// Token: 0x0400028F RID: 655
	Foot_1_L,
	// Token: 0x04000290 RID: 656
	Leg_1_R,
	// Token: 0x04000291 RID: 657
	Knee_1_R,
	// Token: 0x04000292 RID: 658
	Foot_1_R,
	// Token: 0x04000293 RID: 659
	Leg_2_L,
	// Token: 0x04000294 RID: 660
	Knee_2_L,
	// Token: 0x04000295 RID: 661
	Foot_2_L,
	// Token: 0x04000296 RID: 662
	Leg_2_R,
	// Token: 0x04000297 RID: 663
	Knee_2_R,
	// Token: 0x04000298 RID: 664
	Foot_2_R,
	// Token: 0x04000299 RID: 665
	Leg_3_L,
	// Token: 0x0400029A RID: 666
	Knee_3_L,
	// Token: 0x0400029B RID: 667
	Foot_3_L,
	// Token: 0x0400029C RID: 668
	Leg_3_R,
	// Token: 0x0400029D RID: 669
	Knee_3_R,
	// Token: 0x0400029E RID: 670
	Foot_3_R,
	// Token: 0x0400029F RID: 671
	Leg_4_L,
	// Token: 0x040002A0 RID: 672
	Knee_4_L,
	// Token: 0x040002A1 RID: 673
	Foot_4_L,
	// Token: 0x040002A2 RID: 674
	Leg_4_R,
	// Token: 0x040002A3 RID: 675
	Knee_4_R,
	// Token: 0x040002A4 RID: 676
	Foot_4_R,
	// Token: 0x040002A5 RID: 677
	Leg_5_L,
	// Token: 0x040002A6 RID: 678
	Knee_5_L,
	// Token: 0x040002A7 RID: 679
	Foot_5_L,
	// Token: 0x040002A8 RID: 680
	Leg_5_R,
	// Token: 0x040002A9 RID: 681
	Knee_5_R,
	// Token: 0x040002AA RID: 682
	Foot_5_R,
	// Token: 0x040002AB RID: 683
	Leg_6_L,
	// Token: 0x040002AC RID: 684
	Knee_6_L,
	// Token: 0x040002AD RID: 685
	Foot_6_L,
	// Token: 0x040002AE RID: 686
	Leg_6_R,
	// Token: 0x040002AF RID: 687
	Knee_6_R,
	// Token: 0x040002B0 RID: 688
	Foot_6_R,
	// Token: 0x040002B1 RID: 689
	Leg_7_L,
	// Token: 0x040002B2 RID: 690
	Knee_7_L,
	// Token: 0x040002B3 RID: 691
	Foot_7_L,
	// Token: 0x040002B4 RID: 692
	Leg_7_R,
	// Token: 0x040002B5 RID: 693
	Knee_7_R,
	// Token: 0x040002B6 RID: 694
	Foot_7_R,
	// Token: 0x040002B7 RID: 695
	Leg_8_L,
	// Token: 0x040002B8 RID: 696
	Knee_8_L,
	// Token: 0x040002B9 RID: 697
	Foot_8_L,
	// Token: 0x040002BA RID: 698
	Leg_8_R,
	// Token: 0x040002BB RID: 699
	Knee_8_R,
	// Token: 0x040002BC RID: 700
	Foot_8_R,
	// Token: 0x040002BD RID: 701
	Leg_9_L,
	// Token: 0x040002BE RID: 702
	Knee_9_L,
	// Token: 0x040002BF RID: 703
	Foot_9_L,
	// Token: 0x040002C0 RID: 704
	Leg_9_R,
	// Token: 0x040002C1 RID: 705
	Knee_9_R,
	// Token: 0x040002C2 RID: 706
	Foot_9_R,
	// Token: 0x040002C3 RID: 707
	Leg_10_L,
	// Token: 0x040002C4 RID: 708
	Knee_10_L,
	// Token: 0x040002C5 RID: 709
	Foot_10_L,
	// Token: 0x040002C6 RID: 710
	Leg_10_R,
	// Token: 0x040002C7 RID: 711
	Knee_10_R,
	// Token: 0x040002C8 RID: 712
	Foot_10_R,
	// Token: 0x040002C9 RID: 713
	Spine_1,
	// Token: 0x040002CA RID: 714
	Spine_2,
	// Token: 0x040002CB RID: 715
	Spine_3,
	// Token: 0x040002CC RID: 716
	Spine_4,
	// Token: 0x040002CD RID: 717
	Spine_5,
	// Token: 0x040002CE RID: 718
	Spine_6,
	// Token: 0x040002CF RID: 719
	Spine_7,
	// Token: 0x040002D0 RID: 720
	Spine_8,
	// Token: 0x040002D1 RID: 721
	Spine_9,
	// Token: 0x040002D2 RID: 722
	Spine_10,
	// Token: 0x040002D3 RID: 723
	Jiggle_1_L,
	// Token: 0x040002D4 RID: 724
	Jiggle_1_R,
	// Token: 0x040002D5 RID: 725
	Jiggle_2_L,
	// Token: 0x040002D6 RID: 726
	Jiggle_2_R,
	// Token: 0x040002D7 RID: 727
	Jiggle_3_L,
	// Token: 0x040002D8 RID: 728
	Jiggle_3_R,
	// Token: 0x040002D9 RID: 729
	Jiggle_4_L,
	// Token: 0x040002DA RID: 730
	Jiggle_4_R,
	// Token: 0x040002DB RID: 731
	Jiggle_5_L,
	// Token: 0x040002DC RID: 732
	Jiggle_5_R,
	// Token: 0x040002DD RID: 733
	Jiggle_6_L,
	// Token: 0x040002DE RID: 734
	Jiggle_6_R,
	// Token: 0x040002DF RID: 735
	Jiggle_7_L,
	// Token: 0x040002E0 RID: 736
	Jiggle_7_R,
	// Token: 0x040002E1 RID: 737
	Jiggle_8_L,
	// Token: 0x040002E2 RID: 738
	Jiggle_8_R,
	// Token: 0x040002E3 RID: 739
	Jiggle_9_L,
	// Token: 0x040002E4 RID: 740
	Jiggle_9_R,
	// Token: 0x040002E5 RID: 741
	Jiggle_10_L,
	// Token: 0x040002E6 RID: 742
	Jiggle_10_R,
	// Token: 0x040002E7 RID: 743
	Finger_1_1_R,
	// Token: 0x040002E8 RID: 744
	Finger_1_2_R,
	// Token: 0x040002E9 RID: 745
	Finger_1_3_R,
	// Token: 0x040002EA RID: 746
	Finger_2_1_R,
	// Token: 0x040002EB RID: 747
	Finger_2_2_R,
	// Token: 0x040002EC RID: 748
	Finger_2_3_R,
	// Token: 0x040002ED RID: 749
	Finger_3_1_R,
	// Token: 0x040002EE RID: 750
	Finger_3_2_R,
	// Token: 0x040002EF RID: 751
	Finger_3_3_R,
	// Token: 0x040002F0 RID: 752
	Finger_4_1_R,
	// Token: 0x040002F1 RID: 753
	Finger_4_2_R,
	// Token: 0x040002F2 RID: 754
	Finger_4_3_R,
	// Token: 0x040002F3 RID: 755
	Finger_5_1_R,
	// Token: 0x040002F4 RID: 756
	Finger_5_2_R,
	// Token: 0x040002F5 RID: 757
	Finger_5_3_R,
	// Token: 0x040002F6 RID: 758
	Finger_1_1_L,
	// Token: 0x040002F7 RID: 759
	Finger_1_2_L,
	// Token: 0x040002F8 RID: 760
	Finger_1_3_L,
	// Token: 0x040002F9 RID: 761
	Finger_2_1_L,
	// Token: 0x040002FA RID: 762
	Finger_2_2_L,
	// Token: 0x040002FB RID: 763
	Finger_2_3_L,
	// Token: 0x040002FC RID: 764
	Finger_3_1_L,
	// Token: 0x040002FD RID: 765
	Finger_3_2_L,
	// Token: 0x040002FE RID: 766
	Finger_3_3_L,
	// Token: 0x040002FF RID: 767
	Finger_4_1_L,
	// Token: 0x04000300 RID: 768
	Finger_4_2_L,
	// Token: 0x04000301 RID: 769
	Finger_4_3_L,
	// Token: 0x04000302 RID: 770
	Finger_5_1_L,
	// Token: 0x04000303 RID: 771
	Finger_5_2_L,
	// Token: 0x04000304 RID: 772
	Finger_5_3_L,
	// Token: 0x04000305 RID: 773
	Final
}
