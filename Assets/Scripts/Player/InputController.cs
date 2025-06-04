using UnityEngine;

public class InputController : MonoBehaviour
{
    private PlayerInventory _playerInventory;

    private void Awake()
    {
        // �ڳ����в��� PlayerInventory�����賡������һ�����˸ýű���������壩
        _playerInventory = FindObjectOfType<PlayerInventory>();
        if (_playerInventory == null)
        {
            Debug.LogError("[InputController] �Ҳ��� PlayerInventory����ȷ�ϳ�������һ��������� PlayerInventory �����");
        }
    }

    private void Update()
    {
        // �л��������棨ԭ�����ܣ�
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ToggleInventory();
            }
        }

        // ���� F1�����Ի�� Sword01
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_playerInventory == null) return;

            ItemBase sword1 = ItemDatabase.Instance.CreateItem("Sword01");
            if (sword1 != null)
            {
                bool added = _playerInventory.AddItem(sword1);
                if (added)
                    Debug.Log("[InputController] �ѽ� Sword01 ��ӵ�������");
                else
                    Debug.LogWarning("[InputController] �������������ʧ�ܣ��޷���� Sword01��");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase ��δ�ҵ� ID = Sword01 ����������ȷ�� WeaponData ����ȷ���á�");
            }
        }

        // ���� F2�����Ի�� Sword02
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (_playerInventory == null) return;

            ItemBase sword2 = ItemDatabase.Instance.CreateItem("Sword02");
            if (sword2 != null)
            {
                bool added = _playerInventory.AddItem(sword2);
                if (added)
                    Debug.Log("[InputController] �ѽ� Sword02 ��ӵ�������");
                else
                    Debug.LogWarning("[InputController] �������������ʧ�ܣ��޷���� Sword02��");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase ��δ�ҵ� ID = Sword02 ����������ȷ�� WeaponData ����ȷ���á�");
            }
        }

        // ���� F3�����Ի�� Portion01��Restoration portion ����Ʒ��
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (_playerInventory == null) return;

            ItemBase portion = ItemDatabase.Instance.CreateItem("Portion01");
            if (portion != null)
            {
                bool added = _playerInventory.AddItem(portion);
                if (added)
                    Debug.Log("[InputController] �ѽ� Portion01��Restoration portion����ӵ�������");
                else
                    Debug.LogWarning("[InputController] �������������ʧ�ܣ��޷���� Portion01��Restoration portion����");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase ��δ�ҵ� ID = Portion01����ȷ�� ConsumableData ����ȷ���á�");
            }
        }

        // ���� F4�����Ի�� ����ҩˮ��AttackPotion��
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (_playerInventory == null) return;

            ItemBase atkPotion = ItemDatabase.Instance.CreateItem("Portion02");
            if (atkPotion != null)
            {
                bool added = _playerInventory.AddItem(atkPotion);
                if (added)
                    Debug.Log("[InputController] �ѽ� AttackPotion������ҩˮ����ӵ�������");
                else
                    Debug.LogWarning("[InputController] �������������ʧ�ܣ��޷���� AttackPotion������ҩˮ����");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase ��δ�ҵ� ID = Portion02����ȷ�� ConsumableData������ҩˮ������ȷ���á�");
            }
        }

        // ���� F5�����Ի�� ����ҩˮ��DefensePotion��
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (_playerInventory == null) return;

            ItemBase defPotion = ItemDatabase.Instance.CreateItem("Portion03");
            if (defPotion != null)
            {
                bool added = _playerInventory.AddItem(defPotion);
                if (added)
                    Debug.Log("[InputController] �ѽ� DefensePotion������ҩˮ����ӵ�������");
                else
                    Debug.LogWarning("[InputController] �������������ʧ�ܣ��޷���� DefensePotion������ҩˮ����");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase ��δ�ҵ� ID = Portion03����ȷ�� ConsumableData������ҩˮ������ȷ���á�");
            }
        }

        // ���� ���������� F6�����Ի�� Sword03 ���� 
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (_playerInventory == null) return;

            ItemBase sword3 = ItemDatabase.Instance.CreateItem("Sword03");
            if (sword3 != null)
            {
                bool added = _playerInventory.AddItem(sword3);
                if (added)
                    Debug.Log("[InputController] �ѽ� Sword03 ��ӵ�������");
                else
                    Debug.LogWarning("[InputController] �������������ʧ�ܣ��޷���� Sword03��");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase ��δ�ҵ� ID = Sword03 ����������ȷ�� WeaponData ����ȷ���á�");
            }
        }

        // ���� ���������� F7�����Ի�� Armor01 ���� 
        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (_playerInventory == null) return;

            ItemBase armor = ItemDatabase.Instance.CreateItem("Armor01");
            if (armor != null)
            {
                bool added = _playerInventory.AddItem(armor);
                if (added)
                    Debug.Log("[InputController] �ѽ� Armor01�����ף���ӵ�������");
                else
                    Debug.LogWarning("[InputController] �������������ʧ�ܣ��޷���� Armor01�����ף���");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase ��δ�ҵ� ID = Armor01����ȷ�� ArmorData ����ȷ���á�");
            }
        }

        // ���� ���������� F8�����Ի�� Helmet01 ���� 
        if (Input.GetKeyDown(KeyCode.F8))
        {
            if (_playerInventory == null) return;

            ItemBase helmet = ItemDatabase.Instance.CreateItem("Helmet01");
            if (helmet != null)
            {
                bool added = _playerInventory.AddItem(helmet);
                if (added)
                    Debug.Log("[InputController] �ѽ� Helmet01��ͷ������ӵ�������");
                else
                    Debug.LogWarning("[InputController] �������������ʧ�ܣ��޷���� Helmet01��ͷ������");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase ��δ�ҵ� ID = Helmet01����ȷ�� HelmetData ����ȷ���á�");
            }
        }

        // ���� ���������� M �������̵���� ���� 
        if (Input.GetKeyDown(KeyCode.M))
        {
            // ֻ�е��� ShopRoom �������� UIManager �ɹ������� shopCanvas ʱ������Ż��������̵�
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenShop();
            }
        }
    }
}
