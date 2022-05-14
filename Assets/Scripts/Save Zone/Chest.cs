using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    private bool inRange = false;
    private bool isTriggered = false;
    private Consumables consumables;
    //{itemAmount, chance}
    private Dictionary<int, float> chancesForDropAmount = new Dictionary<int, float>
    {
        { 0, 40 },
        { 1, 25 },
        { 2, 20 },
        { 3, 10 },
        { 4, 5 },
    };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ServiceInfo.PlayerTag))
            inRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(ServiceInfo.PlayerTag))
            inRange = false;
    }

    void Start()
    {
        consumables = Player.instanse.GetComponent<Consumables>();
    }

    private void Update()
    {
        if (!isTriggered && inRange && Input.GetKeyDown(KeyCode.E))
        {
            //���� �������������� �������
            var consumabelsList = consumables.GetType().GetProperties(
                System.Reflection.BindingFlags.Public|
                System.Reflection.BindingFlags.Instance|
                System.Reflection.BindingFlags.DeclaredOnly);
            var anyAmountIncreased = false;
            foreach (var propertyInfo in consumabelsList) {
                var chance = UnityEngine.Random.value * 100;
                var consumableObj = propertyInfo.GetValue(consumables);
                //Debug.Log("\n[+] Name: " + propertyInfo.Name + "\n[+] Value: " + consumableObj);
                //�������� �� ��� ��������
                if (!consumableObj.GetType().Equals(1.GetType())) {
                    Debug.Log("Property type is not int!");
                    continue;
                }
                //������ �� �� ����� ��������� � ����� ����������, ������� ������ ������
                var consumableCount = (int) consumableObj;
                Debug.Log("\nCurrent generated number " + chance);
                for (int i = 0; i <= 4; i++) 
                {
                    if (chance <= chancesForDropAmount[i]) 
                    {
                        propertyInfo.SetValue(consumables, consumableCount + i);
                        anyAmountIncreased = true;
                        Debug.Log("\nAdded " + i + " to " + propertyInfo.Name);
                        break;
                    }
                    chance -= chancesForDropAmount[i];
                }
                //���� ����� �� ����� ������ ������ ���������� ���������� ��������� ���-�� (����� ����� �������� �� ��� ������)
                if (!anyAmountIncreased) 
                {
                    Debug.Log("Unlucky random! (For each property 0 amount increased)");
                    consumabelsList[(int)(UnityEngine.Random.value * 100) % 5].SetValue(consumables, consumableCount+1+((int)(UnityEngine.Random.value * 100) % 5));
                }
                //propertyInfo.SetValue(consumables, consumableCount + 99);

            }
            //Debug.Log("Created list of consumabels with lenght: " + consumabelsList.Length);
            // ��� ���������� ������
            isTriggered = true;
            ServiceInfo.CheckpointConditionDone = true;

        }
    }
}