using NUnit.Framework;
using UnityEditor;
using Voidex.Badge.Runtime;
using Voidex.Badge.Sample;
using Voidex.Trie;

public class BadgesTest
{
    [SetUp]
    public void Setup()
    {
        BadgeMessaging.Initialize(new MessagePipeMessaging());
    }

    [TearDown]
    public void ClearUp()
    {
        BadgeMessaging.Initialize(new MessagePipeMessaging());
    }

    [Test]
    public void Test_Add_Badge_01()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        if (graph.Length == 0)
        {
            Assert.Fail("No BadgeGraph found");
        }

        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);
        badgeNotification.AddBadge("Root|Mails|Secrete", 1);
        var badge = badgeNotification.GetBadge("Root|Mails|Secrete");
        Assert.NotNull(badge);
        Assert.AreEqual(1, badge.value);

        badgeNotification.AddBadge("Root|Mails|System|0", 10);
        badge = badgeNotification.GetBadge("Root|Mails|System|0");
        Assert.NotNull(badge);
        Assert.AreEqual(10, badge.value);
    }

    [Test]
    public void Test_Add_Badge_02()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);
        badgeNotification.AddBadge("Root|Characters|ValueUp", 5);
        var badge = badgeNotification.GetBadge("Root|Characters|ValueUp");
        Assert.NotNull(badge);
        Assert.AreEqual(5, badge.value);
    }

    [Test]
    public void Test_Update_Badge_01()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);

        badgeNotification.AddBadge("Root|Mails|Rewarded", 1);
        badgeNotification.UpdateBadge("Root|Mails|Rewarded", -1);
        var badge = badgeNotification.GetBadge("Root|Mails|Rewarded");
        Assert.AreEqual(0, badge.value);
    }

    [Test]
    public void Test_Update_Badge_02()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);
        badgeNotification.AddBadge("Root|Mails|Secrete", 1);
        badgeNotification.UpdateBadge("Root|Mails|Secrete", 1);
        var badge = badgeNotification.GetBadge("Root|Mails|Secrete");
        Assert.AreEqual(2, badge.value);
    }

    [Test]
    public void Test_Update_Badge_03()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);

        badgeNotification.AddBadge("Root|Quests|Daily|0", 1);
        badgeNotification.UpdateBadge("Root|Quests|Daily|0", -1);
        var badge = badgeNotification.GetBadge("Root|Quests|Daily|0");
        Assert.AreEqual(0, badge.value);
    }

    [Test]
    public void Test_Get_Badge_Value_01()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);

        badgeNotification.AddBadge("Root|Mails|Secrete", 1);
        Assert.AreEqual(1, badgeNotification.GetBadgeValue("Root|Mails|Secrete"));

        badgeNotification.UpdateBadge("Root|Mails|Rewarded", -11);
        var badge = badgeNotification.GetBadgeValue("Root|Mails|Rewarded");
        Assert.AreEqual(0, badge);

        badgeNotification.UpdateBadge("Root|Mails|System", 4);
        badge = badgeNotification.GetBadgeValue("Root|Mails|System");
        Assert.AreEqual(4, badge);
        var badge2 = badgeNotification.GetBadge("Root|Mails|Rewarded");
        Assert.AreEqual(0, badge2.value);
        badgeNotification.UpdateBadge("Root|Mails|Rewarded", 1);

        badge = badgeNotification.GetBadgeValue("Root|Mails");
        Assert.AreEqual(6, badge);
    }

    [Test]
    public void Test_Get_Badge_Value_02()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);

        badgeNotification.UpdateBadge("Root|Mails|System", 1);
        Assert.AreEqual(1, badgeNotification.GetBadgeValue("Root|Mails|System"));
        badgeNotification.UpdateBadge("Root|Mails|Rewarded", 4);
        Assert.AreEqual(4, badgeNotification.GetBadgeValue("Root|Mails|Rewarded"));

        var badge = badgeNotification.GetBadgeValue("Root|Mails");
        Assert.AreEqual(5, badge);
    }

    [Test]
    public void Test_Update_Badges_01()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);

        var prefix = "Root|Characters|1|UpE";
        badgeNotification.UpdateBadges(prefix, 1);
        var badge = badgeNotification.GetBadge(prefix);
        Assert.AreEqual(6, badge.value);
        var root = badgeNotification.GetBadgeValue("Root");
        Assert.AreEqual(6, root);
        var characters = badgeNotification.GetBadgeValue("Root|Characters");
        Assert.AreEqual(6, characters);
        var slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Sword");
        Assert.AreEqual(1, slot1);
        var slot2 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Horse");
        Assert.AreEqual(1, slot2);
        var slot3 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Armor");
        Assert.AreEqual(1, slot3);
        var slot4 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Shield");
        Assert.AreEqual(1, slot4);
        var slot5 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Helmet");
        Assert.AreEqual(1, slot5);
        var slot6 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Accessory");
        Assert.AreEqual(1, slot6);

        badgeNotification.UpdateBadges("Root|Characters|1|UpE|Sword", 1);
        slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Sword");
        Assert.AreEqual(2, slot1);
        var upE = badgeNotification.GetBadgeValue("Root|Characters|1|UpE");
        Assert.AreEqual(7, upE);
        badgeNotification.UpdateBadges("Root|Characters|1|UpE|Horse", -1);
        slot2 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Horse");
        Assert.AreEqual(0, slot2);
        upE = badgeNotification.GetBadgeValue("Root|Characters|1|UpE");
        Assert.AreEqual(6, upE);
    }

    [Test]
    public void Test_Update_Badges_02()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);


        var prefix = "Root|Characters";
        badgeNotification.UpdateBadges(prefix, 1);
        var badge = badgeNotification.GetBadgeValue(prefix);
        Assert.AreEqual(26, badge);

        var root = badgeNotification.GetBadgeValue("Root");
        Assert.AreEqual(26, root);

        var slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Sword");
        Assert.AreEqual(1, slot1);

        var upC = badgeNotification.GetBadgeValue("Root|Characters|1|UpC");
        Assert.AreEqual(1, upC);
    }

    [Test]
    public void Test_Update_Badges_With_Postfix_01()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);

        var postfix = "Sword";
        badgeNotification.UpdateBadges("Root|Characters", postfix, 2);
        var upE_Slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Sword");
        Assert.AreEqual(2, upE_Slot1);
        var equip_Slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|Equip|Sword");
        Assert.AreEqual(2, equip_Slot1);
        var characters = badgeNotification.GetBadgeValue("Root|Characters");
        Assert.AreEqual(8, characters);
        var root = badgeNotification.GetBadgeValue("Root");
        Assert.AreEqual(8, root);
    }

    [Test]
    public void Test_Update_Badges_03()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);

        string prefix = "Root|Characters";
        string postfix = "Equip|Sword";
        var node = badgeNotification.GetTrieNode(prefix);
        badgeNotification.UpdateNodeAndChildren(node, 1);
        Assert.AreEqual(26, badgeNotification.GetBadgeValue("Root|Characters"));

        var characters = badgeNotification.GetBadgeValue("Root|Characters");
        Assert.AreEqual(26, characters);

        var upE_Slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|UpE|Sword");
        Assert.AreEqual(1, upE_Slot1);
        var equip_Slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|Equip|Sword");
        Assert.AreEqual(1, equip_Slot1);

        characters = badgeNotification.GetBadgeValue("Root|Characters");
        Assert.AreEqual(26, characters);

        badgeNotification.UpdateBadges(prefix, postfix, 1);
        equip_Slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|Equip|Sword");
        Assert.AreEqual(2, equip_Slot1);
        characters = badgeNotification.GetBadgeValue("Root|Characters");
        Assert.AreEqual(28, characters);


        badgeNotification.UpdateBadges(prefix, postfix, -1);
        equip_Slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|Equip|Sword");
        Assert.AreEqual(1, equip_Slot1);
        characters = badgeNotification.GetBadgeValue("Root|Characters");
        Assert.AreEqual(26, characters);
        
        badgeNotification.UpdateBadges(prefix, postfix, -1);
        equip_Slot1 = badgeNotification.GetBadgeValue("Root|Characters|1|Equip|Sword");
        Assert.AreEqual(0, equip_Slot1);
        characters = badgeNotification.GetBadgeValue("Root|Characters");
        Assert.AreEqual(24, characters);
    }

    [Test]
    public void Test_Set_Badge_Value_01()
    {
        //find graph using AssetDatabase
        var graph = AssetDatabase.FindAssets("t:BadgeGraph");
        //load graph using AssetDatabase
        var badgeGraph = AssetDatabase.LoadAssetAtPath<BadgeGraph>(AssetDatabase.GUIDToAssetPath(graph[0]));
        BadgeNotification badgeNotification = new BadgeNotification(badgeGraph);

        badgeNotification.SetBadgeValue("Root|Characters|1|Equip|Sword", 5);
        var badge = badgeNotification.GetBadgeValue("Root|Characters|1|Equip|Sword");
        Assert.AreEqual(5, badge);
        badgeNotification.UpdateBadges("Root|Characters", "UpC", 1);
        badge = badgeNotification.GetBadgeValue("Root|Characters|1|UpC");
        Assert.AreEqual(1, badge);
        
        Assert.AreEqual(7, badgeNotification.GetBadgeValue("Root|Characters"));
    }
}