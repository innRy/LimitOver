using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ���ǉ��@�FUI�i�e�L�X�g�Ȃǁj�𑀍삷�邽�߂ɐ�΂ɕK�v�I

public class PlayerScript : MonoBehaviour
{
    public GameObject goalUI;
    public Text countUI; // ���ǉ��A�F������\������e�L�X�g�����锠

    public int fragmentCount = 0;
    public bool hasKey = false;

    void Start()
    {
        if (goalUI != null)
        {
            goalUI.SetActive(false);
        }

        // ���ǉ��B�F�Q�[���J�n���Ɂu0 / 5�v�ƕ\��������
        UpdateCountUI();
    }

    void OnTriggerEnter(Collider other)
    {
        // �@ �G�ꂽ���肪�uFragment�i������j�v�������ꍇ
        if (other.CompareTag("Fragment"))//���̏�����ς��邱�ƂŌ��̂�����̏�����ς��邱�Ƃ��ł���
        {
            fragmentCount++; // ������̐���1���₷
            Destroy(other.gameObject); // �E���������������

            // ���ǉ��C�F��������E�����тɁA��ʂ̐���������������
            UpdateCountUI();

            Debug.Log("��������Q�b�g�I ����: " + fragmentCount + "��");

            // �����炪5�W�܂�A���܂������������Ă��Ȃ��ꍇ
            if (fragmentCount >= 5 && hasKey == false)  //���̐����܂łɕK�v�Ȃ�����̌���ύX�ł���
            {
                hasKey = true; // ��������������Ԃɂ���
                Debug.Log("5�W�܂����I�������������I");
            }
        }

        // �A �G�ꂽ���肪�uGoal�i�S�[���j�v�������ꍇ
        if (other.gameObject.name == "Goal")    //���̏�����ς��邱�ƂŐG���I�u�W�F�N�g��ύX�ł���
        {
            if (hasKey == true)
            {
                Debug.Log("�S�[���I�I");

                if (goalUI != null)
                {
                    goalUI.SetActive(true);
                }
            }
            else
            {
                int needCount = 5 - fragmentCount;
                Debug.Log("�S�[������ɂ͌����K�v���I ���Ƃ����炪 " + needCount + "�� �K�v���I");
            }
        }
    }

    // ���ǉ��D�F���������������鏈���i�����Ȃ�̂ł܂Ƃ߂܂����j
    void UpdateCountUI()
    {
        // ���̒��ɂ����ƃe�L�X�gUI�������Ă���Ώ���������
        if (countUI != null)
        {
            // .text ���g���ƁA��ʂ̕��������R�ɕύX�ł��܂�
            countUI.text = "鍵のかけら: " + fragmentCount + " / 5";
        }
    }
}