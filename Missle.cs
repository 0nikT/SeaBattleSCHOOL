using UnityEngine;

namespace InfiniteVox
{
    public class Missle : MonoBehaviour
    {
        [SerializeField] private Transform _spawnTransform;
        [SerializeField] private MissleObj _missleObj;
        [SerializeField] private float _angle;

        private Transform _target;

        private void Update() => _spawnTransform.localEulerAngles = new Vector3(-_angle, 0f, 0f);

        public void Shot(Transform target)
        {
            _target = target;

            Invoke(nameof(Shot), 0.001f);
        }

        private void Shot()
{
    Vector3 direction = _target.position - _spawnTransform.position; // Направление к цели
    Vector3 rotateDirection = new Vector3(direction.x, 0f, direction.z); // Без вертикальной компоненты

    transform.rotation = Quaternion.LookRotation(rotateDirection, Vector3.up); // Направляем снаряд на цель

    float x = rotateDirection.magnitude; // Горизонтальная дистанция
    float y = direction.y; // Вертикальная дистанция

    // Мы больше не будем использовать угол и расчёт баллистики, т.к. снаряд будет лететь как торпеда
    float speed = 100f; // Задаём скорость торпеды (настраиваем значение для вашего случая)

    // Создаём снаряд
    MissleObj missleObj = Instantiate(_missleObj.gameObject, _spawnTransform.position, Quaternion.identity).GetComponent<MissleObj>();
    Rigidbody rb = missleObj.GetComponent<Rigidbody>();

    // Устанавливаем постоянную скорость на весь путь
    Vector3 velocity = direction.normalized * speed; 

    // Убираем гравитацию для снаряда, чтобы он не падал
    rb.useGravity = false;
    
    // Применяем скорость
    rb.velocity = velocity;

    missleObj.Target = _target.gameObject; // Устанавливаем цель снаряда
}

    }
}