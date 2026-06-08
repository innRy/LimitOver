using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ���ǉ��@�FUI�i�e�L�X�g�Ȃǁj�𑀍삷�邽�߂ɐ�΂ɕK�v�I                      // 1. 上部にこれを追加
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public GameObject goalUI;

// 2. 変数の型を Text から TextMeshProUGUI に変更
    public TextMeshProUGUI countUI;

    public Transform spawnPoint;    // 上で作った空のGameObjectをアサイン

  

    public int fragmentCount = 0;
    public bool hasKey = false;

    void Start()
    {
        if (spawnPoint != null)
        {
            this.transform.position = spawnPoint.position;
            this.transform.rotation = spawnPoint.rotation;
        }
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

            // �����炪5�W�܂�A���܂������������Ă��Ȃ��ꍇ
            if (fragmentCount >= 5 && hasKey == false)  //���̐����܂łɕK�v�Ȃ�����̌���ύX�ł���
            {
                hasKey = true; // ��������������Ԃɂ���
            }
        }

        // �A �G�ꂽ���肪�uGoal�i�S�[���j�v�������ꍇ
        if (other.gameObject.name == "Goal")    //���̏�����ς��邱�ƂŐG���I�u�W�F�N�g��ύX�ł���
        {
            if (hasKey == true)
            {

                if (goalUI != null)
                {
                    goalUI.SetActive(true);
                    StartCoroutine(WaitAndProcessCoroutine());
                    goalUI.SetActive(false);
                }
            }
            else
            {
                int needCount = 5 - fragmentCount;
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
            countUI.text = "鍵のかけら" + fragmentCount + " / 5";
        }
    }
    IEnumerator WaitAndProcessCoroutine()
    {
        // なにか処理
        yield return new WaitForSeconds(4f); // ← IEnumerator の中ならエラーにならない！

        if (goalUI != null)
        {
            goalUI.SetActive(false);
        }
        // なにか処理
    }
}