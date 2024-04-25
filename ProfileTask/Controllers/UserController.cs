using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VKProfileTask.Entities;
using VKProfileTask.Models;
using VKProfileTask.UnitOfWorks;

namespace VKProfileTask.Controllers;

// Атрибуты, указывающие, что этот класс является контроллером API, с базовым маршрутом для доступа к методам
[ApiController]
[Route("API/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger; // Логгер для записи информации и ошибок
    private readonly PostgresUnitOfWork _postgresWork; // Unit of Work для работы с базой данных

    // Конструктор класса, инициализирует логгер и unit of work
    public UserController(ILogger<UserController> logger, DataContext dataContext)
    {
        _logger = logger;
        _postgresWork = new PostgresUnitOfWork(dataContext);
    }

    // Метод для добавления пользователя, принимает объект пользователя
    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserForAdding userForAdding)
    {
        var stopwatch = new Stopwatch(); // Таймер для измерения времени операции
        if (userForAdding.GroupCode is < 0 or > 1) // Проверка допустимости кода группы пользователя
        {
            _logger.LogError("Couldn\'t add a new user. Bad user group code = {GroupCode}.", userForAdding.GroupCode);
            return BadRequest(); // Возврат ошибки, если код группы некорректен
        }

        // Создание состояния и группы пользователя
        var userState = new UserStateEntity(StateCode.Undefined, userForAdding.StateDescription);
        var userGroup = new UserGroupEntity((GroupCode)userForAdding.GroupCode, userForAdding.GroupDescription);
        // Создание сущности пользователя
        var user = new UserEntity(userForAdding.Login, userForAdding.Password, userGroup.Id, userState.Id);
        // Попытка добавить пользователя в базу данных
        if (!await _postgresWork.AddUserAsync(user, (GroupCode)userForAdding.GroupCode))
        {
            _logger.LogError(
                "Couldn\'t add a new user. The user with login = {Login} is already being added{OrAdminAlreadyExists}",
                userForAdding.Login,
                (GroupCode)userForAdding.GroupCode == GroupCode.User ? "." : " or admin already exists.");
            return BadRequest(); // Возврат ошибки, если такой пользователь уже существует
        }

        // Добавление группы и состояния пользователя в базу данных
        await _postgresWork.Groups.AddAsync(userGroup);
        await _postgresWork.States.AddAsync(userState);

        stopwatch.Stop(); // Остановка таймера
        await Task.Delay((int)(5000 -
                               stopwatch.ElapsedMilliseconds)); // Искусственная задержка для демонстрации асинхронности

        // Активация состояния пользователя
        userState.Code = StateCode.Active;
        await _postgresWork.States.SaveAsync();

        _logger.LogInformation("Successfully added a new user with id = {UserId}.", user.Id);

        return Ok(); // Возврат успешного статуса
    }

    // Метод для удаления пользователя по логину
    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromBody] UserWithLogin userWithLogin)
    {
        if (!await _postgresWork.DeleteUserByLoginAsync(userWithLogin.Login))
        {
            _logger.LogError("Couldn\'t delete the user. The user with id = {Login} doesn\'t exist.",
                userWithLogin.Login);
            return BadRequest(); // Возврат ошибки, если пользователь не найден
        }

        _logger.LogInformation("Successfully deleted user with id = {Login}.", userWithLogin.Login);

        return Ok(); // Возврат успешного статуса
    }

    // Метод для получения пользователя по логину
    [HttpGet]
    public async Task<IActionResult> GetUser([FromBody] UserWithLogin userWithLogin)
    {
        var user = await _postgresWork.GetUserByLoginAsync(userWithLogin.Login);
        if (user == null)
        {
            _logger.LogError("Couldn\'t find the user with id = {Login}.", userWithLogin.Login);
            return NotFound(); // Возвратстатуса "не найден", если пользователь не найден в базе данных
        }

        _logger.LogInformation("Successfully found the user with id = {Login}.", userWithLogin.Login);
        return Ok(user); // Возврат успешного статуса с данными пользователя
    }

// Метод для получения списка всех пользователей, разделённых по страницам
    [HttpGet]
    public async Task<IActionResult> GetAllUsersByPages([FromBody] Page page)
    {
        if (page is
            {
                PageNumber: > 0, PageCapacity: > 0
            }) // Проверка корректности номера страницы и количества элементов на странице
            return Ok(await _postgresWork.GetFullInfoOfAllUsersAsync(page.PageNumber,
                page.PageCapacity)); // Возврат списка пользователей по страницам
        _logger.LogError(
            "Couldn\'t get users. Bad page number = {PagePageNumber} or page capacity = {PagePageCapacity}.",
            page.PageNumber, page.PageCapacity);
        return BadRequest(); // Возврат ошибки, если данные страницы некорректны
    }

// Метод для получения списка всех пользователей без разбиения по страницам
    [HttpGet]
    public async Task<IActionResult> GetAllUsers() =>
        Ok(await _postgresWork.GetFullInfoOfAllUsersAsync()); // Возврат списка всех пользователей
}
