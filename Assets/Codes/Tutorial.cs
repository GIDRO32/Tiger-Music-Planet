using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject[] panels;
    public AudioSource SFX;
    public AudioClip Scrolling;
    private int currentPage = 0;

    void Start()
    {
        ShowPage(currentPage);
    }

    void Update()
    {
        // Логіка перемикання сторінок туторіалу
    }

    // Функція для показу конкретної сторінки
    void ShowPage(int pageIndex)
    {
        // Виключаємо всі сторінки
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        // Перевіряємо, чи індекс не виходить за межі масиву
        if (pageIndex >= 0 && pageIndex < panels.Length)
        {
            // Включаємо тільки обрану сторінку
            panels[pageIndex].SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid page index");
        }
    }

    // Функція для перемикання наступної сторінки
    public void NextPage()
    {
        currentPage++;
        SFX.PlayOneShot(Scrolling);
        if (currentPage >= panels.Length)
        {
            currentPage = 0; // Зациклюємо на першу сторінку, якщо досягнуто кінця
        }
        ShowPage(currentPage);
    }

    // Функція для перемикання попередньої сторінки
    public void PreviousPage()
    {
        SFX.PlayOneShot(Scrolling);
        currentPage--;
        if (currentPage < 0)
        {
            currentPage = panels.Length - 1; // Переходимо на останню сторінку, якщо досягнуто початку
        }
        ShowPage(currentPage);
    }
}
