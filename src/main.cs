/**
Nguyen Phuc Truong An
202419022
*/

using System.Text;
using LabW07.Enums;
using LabW07.Exceptions;
using LabW07.Interfaces;
using LabW07.Models;
using LabW07.Repositories;
using LabW07.Services;
using LabW07.Strategies;

namespace LabW07;

public static class Program
{
    private static int passed;
    private static int failed;

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // ---------- Dữ liệu mẫu ----------
        var pr01 = new ProjectorRemote("PR01", "Remote máy chiếu Epson", "Kho A", 85);
        var pr02 = new ProjectorRemote("PR02", "Remote máy chiếu Sony", "Kho A", 15);   // pin < 20 → chưa sẵn sàng
        var pr03 = new ProjectorRemote("PR03", "Remote máy chiếu Panasonic", "Kho B", 60);

        var ac01 = new AirConditionerRemote("AC01", "Remote Daikin", "Kho A", new[] { "A201", "A202" });
        var ac02 = new AirConditionerRemote("AC02", "Remote Panasonic", "Kho B", new[] { "A203", "B101" });

        var mc01 = new WirelessMicrophone("MC01", "Micro Shure", "Kho A", 25, 90);      // pin < 30 → chưa sẵn sàng
        var mc02 = new WirelessMicrophone("MC02", "Micro Sennheiser", "Kho A", 70, 80);
        var mc03 = new WirelessMicrophone("MC03", "Micro AKG", "Kho B", 90, 35);        // tín hiệu < 40 → chưa sẵn sàng

        // Cùng một lớp generic InMemoryRepository<T> dùng cho 3 loại thiết bị.
        IRepository<ProjectorRemote> projectorRepository = new InMemoryRepository<ProjectorRemote>();
        IRepository<AirConditionerRemote> airConditionerRepository = new InMemoryRepository<AirConditionerRemote>();
        IRepository<WirelessMicrophone> microphoneRepository = new InMemoryRepository<WirelessMicrophone>();

        // ---------- 1. Thêm thiết bị ----------
        Section("1. Thêm thiết bị vào repository");
        projectorRepository.Add(pr01);
        projectorRepository.Add(pr02);
        projectorRepository.Add(pr03);
        airConditionerRepository.Add(ac01);
        airConditionerRepository.Add(ac02);
        microphoneRepository.Add(mc01);
        microphoneRepository.Add(mc02);
        microphoneRepository.Add(mc03);

        PrintAll("Máy chiếu", projectorRepository.GetAll());
        PrintAll("Điều hòa", airConditionerRepository.GetAll());
        PrintAll("Micro", microphoneRepository.GetAll());

        Check("Kho máy chiếu có 3 thiết bị", projectorRepository.GetAll().Count == 3);
        Check("Kho điều hòa có 2 thiết bị", airConditionerRepository.GetAll().Count == 2);
        Check("Kho micro có 3 thiết bị", microphoneRepository.GetAll().Count == 3);
        Check("Trạng thái ban đầu là Available",
            projectorRepository.GetAll().All(d => d.Status == DeviceStatus.Available));
        ExpectThrows<NotSupportedException>("GetAll() không cho sửa collection nội bộ",
            () => ((ICollection<ProjectorRemote>)projectorRepository.GetAll()).Add(pr01));
        ExpectThrows<ArgumentNullException>("Add(null) bị từ chối",
            () => projectorRepository.Add(null!));
        ExpectThrows<ArgumentOutOfRangeException>("Không tạo được máy chiếu có pin 150",
            () => new ProjectorRemote("PR99", "Lỗi", "Kho A", 150));
        ExpectThrows<ArgumentException>("Không tạo được thiết bị có mã rỗng",
            () => new WirelessMicrophone(" ", "Lỗi", "Kho A", 50, 50));

        // ---------- 2. Thêm trùng mã ----------
        Section("2. Thêm hai thiết bị trùng mã");
        var duplicate = new ProjectorRemote("PR01", "Remote trùng mã", "Kho C", 50);
        var duplicateException = ExpectThrows<DuplicateIdException>("Thêm PR01 lần hai → DuplicateIdException",
            () => projectorRepository.Add(duplicate));
        Check("DuplicateId == \"PR01\"", duplicateException?.DuplicateId == "PR01");
        Check("Kho vẫn chỉ có 3 máy chiếu", projectorRepository.GetAll().Count == 3);

        // ---------- 3. Tìm theo mã ----------
        Section("3. Tìm thiết bị theo mã");
        AirConditionerRemote? foundAc = airConditionerRepository.FindById("AC02");
        Console.WriteLine($"   FindById(\"AC02\") → {foundAc}");
        Check("FindById(\"AC02\") trả về đúng AC02", ReferenceEquals(foundAc, ac02));
        Check("FindById(\"XX\") trả về null", airConditionerRepository.FindById("XX") is null);

        // ---------- 4. Lọc thiết bị sẵn sàng ----------
        Section("4. Lọc các thiết bị sẵn sàng");
        var readyProjectors = projectorRepository.FindAll(d => d.IsReadyForUse());
        var readyMicrophones = microphoneRepository.FindAll(d => d.IsReadyForUse());
        Console.WriteLine($"   Máy chiếu sẵn sàng: {Ids(readyProjectors)}");
        Console.WriteLine($"   Micro sẵn sàng    : {Ids(readyMicrophones)}");
        Check("Máy chiếu sẵn sàng = PR01, PR03", Ids(readyProjectors) == "PR01, PR03");
        Check("Micro sẵn sàng = MC02", Ids(readyMicrophones) == "MC02");

        // ---------- 5. Chọn máy chiếu điểm cao nhất ----------
        Section("5. Chọn điều khiển máy chiếu có điểm cao nhất");
        var highestProjector = new HighestPriorityStrategy<ProjectorRemote>(d => d.GetDefaultPriorityScore());

        var input = new List<ProjectorRemote> { pr03, pr02, pr01 };
        highestProjector.Select(input);
        Check("Strategy không thay đổi danh sách đầu vào", Ids(input) == "PR03, PR02, PR01");

        var projectorService = new AllocationService<ProjectorRemote>(projectorRepository, highestProjector);
        ProjectorRemote selectedProjector = projectorService.Allocate(device => true);
        Console.WriteLine($"   Được chọn: {selectedProjector}");
        Check("Chọn PR01 (pin 85)", selectedProjector.Id == "PR01");

        // ---------- 6. Chọn điều hòa theo phòng ----------
        Section("6. Chọn điều khiển điều hòa tương thích với phòng A203");
        var airConditionerService = new AllocationService<AirConditionerRemote>(
            airConditionerRepository, new FirstAvailableStrategy<AirConditionerRemote>());
        AirConditionerRemote selectedAc = airConditionerService.Allocate(device => device.IsCompatibleWith("A203"));
        Console.WriteLine($"   Được chọn: {selectedAc}");
        Check("Chọn AC02 (tương thích A203)", selectedAc.Id == "AC02");
        Check("IsCompatibleWith không phân biệt hoa/thường (\"a201\")", ac01.IsCompatibleWith("a201"));

        // ---------- 7. Chọn micro đầu tiên sẵn sàng ----------
        Section("7. Chọn micro đầu tiên sẵn sàng");
        var microphoneService = new AllocationService<WirelessMicrophone>(
            microphoneRepository, new FirstAvailableStrategy<WirelessMicrophone>());
        WirelessMicrophone selectedMic = microphoneService.Allocate();
        Console.WriteLine($"   Được chọn: {selectedMic}");
        Check("Chọn MC02 (MC01 thiếu pin, MC03 yếu tín hiệu)", selectedMic.Id == "MC02");

        // ---------- 8. Đổi chiến lược lúc chạy ----------
        Section("8. Đổi chiến lược trong thời gian chạy");
        IRepository<ProjectorRemote> roomBRepository = new InMemoryRepository<ProjectorRemote>();
        roomBRepository.Add(new ProjectorRemote("PR11", "Remote B-1", "Kho C", 60));
        roomBRepository.Add(new ProjectorRemote("PR12", "Remote B-2", "Kho C", 95));
        roomBRepository.Add(new ProjectorRemote("PR13", "Remote B-3", "Kho C", 75));
        PrintAll("Kho C", roomBRepository.GetAll());

        var roomBService = new AllocationService<ProjectorRemote>(
            roomBRepository, new FirstAvailableStrategy<ProjectorRemote>());
        ProjectorRemote first = roomBService.Allocate();
        Console.WriteLine($"   [{roomBService.CurrentStrategy}] → {first.Id}");
        Check("FirstAvailable chọn PR11", first.Id == "PR11");

        roomBService.ChangeStrategy(new HighestPriorityStrategy<ProjectorRemote>(d => d.GetDefaultPriorityScore()));
        ProjectorRemote highest = roomBService.Allocate();
        Console.WriteLine($"   [{roomBService.CurrentStrategy}] → {highest.Id}");
        Check("Đổi sang HighestPriority chọn PR12 (pin 95)", highest.Id == "PR12");

        roomBService.ChangeStrategy(new HighestPriorityStrategy<ProjectorRemote>(d => -d.BatteryLevel));
        roomBService.Return("PR11");
        ProjectorRemote lowest = roomBService.Allocate();
        Console.WriteLine($"   [Strategy pin thấp nhất, sau khi trả PR11] → {lowest.Id}");
        Check("Hàm tính điểm tùy biến (pin thấp nhất) chọn PR11", lowest.Id == "PR11");
        ExpectThrows<ArgumentNullException>("ChangeStrategy(null) bị từ chối",
            () => roomBService.ChangeStrategy(null!));

        // ---------- 9. Không có ứng viên phù hợp ----------
        Section("9. Cấp phát khi không có ứng viên phù hợp");
        ExpectThrows<NoSuitableItemException>("Điều hòa cho phòng Z999 → NoSuitableItemException",
            () => airConditionerService.Allocate(device => device.IsCompatibleWith("Z999")));
        ExpectThrows<NoSuitableItemException>("Micro: không còn micro nào sẵn sàng → NoSuitableItemException",
            () => microphoneService.Allocate());
        ExpectThrows<NoSuitableItemException>("HighestPriorityStrategy với danh sách rỗng",
            () => highestProjector.Select(Array.Empty<ProjectorRemote>()));
        ExpectThrows<NoSuitableItemException>("FirstAvailableStrategy với danh sách rỗng",
            () => new FirstAvailableStrategy<int>().Select(Array.Empty<int>()));

        // ---------- 10. Thiết bị được chọn chuyển sang Borrowed ----------
        Section("10. Thiết bị được chọn chuyển sang Borrowed");
        Console.WriteLine($"   {pr01}");
        Check("PR01.Status == Borrowed", pr01.Status == DeviceStatus.Borrowed);
        Check("PR01 không còn sẵn sàng", !pr01.IsReadyForUse());

        ProjectorRemote next = projectorService.Allocate();
        Console.WriteLine($"   Lần cấp phát tiếp theo → {next.Id}");
        Check("Lần sau không chọn lại PR01 mà chọn PR03", next.Id == "PR03");

        var notReady = ExpectThrows<DeviceNotReadyException>("Borrow() PR01 lần hai → DeviceNotReadyException",
            () => pr01.Borrow());
        Check("Exception chứa DeviceId = PR01, CurrentStatus = Borrowed",
            notReady?.DeviceId == "PR01" && notReady.CurrentStatus == DeviceStatus.Borrowed);

        projectorService.Return("PR01");
        Check("Return(\"PR01\") → Available", pr01.Status == DeviceStatus.Available);
        ExpectThrows<DeviceNotReadyException>("Return() khi không Borrowed → DeviceNotReadyException",
            () => pr01.Return());
        ExpectThrows<KeyNotFoundException>("Return(\"XX\") → KeyNotFoundException",
            () => projectorService.Return("XX"));

        pr01.SendToMaintenance();
        Check("PR01 đang bảo trì không sẵn sàng", !pr01.IsReadyForUse());
        ExpectThrows<NoSuitableItemException>("PR01 bảo trì, PR02 yếu pin, PR03 đã mượn → không còn ứng viên",
            () => projectorService.Allocate());
        pr01.CompleteMaintenance();
        Check("PR01 bảo trì xong → Available", pr01.Status == DeviceStatus.Available);

        // ---------- 11. Repository tổng quát dùng cho nhiều loại ----------
        Section("11. Repository tổng quát dùng cho nhiều loại thiết bị");
        Check("InMemoryRepository<ProjectorRemote> và <WirelessMicrophone> cùng một lớp generic",
            projectorRepository.GetType().GetGenericTypeDefinition()
            == microphoneRepository.GetType().GetGenericTypeDefinition());

        // Kho chứa lẫn nhiều loại: đa hình quyết định IsReadyForUse()/GetDefaultPriorityScore().
        IRepository<TeachingDevice> mixedRepository = new InMemoryRepository<TeachingDevice>();
        mixedRepository.Add(new ProjectorRemote("X-PR", "Remote máy chiếu", "Kho D", 40));
        mixedRepository.Add(new WirelessMicrophone("X-MC", "Micro", "Kho D", 100, 90));
        mixedRepository.Add(new AirConditionerRemote("X-AC", "Remote điều hòa", "Kho D", Array.Empty<string>()));
        PrintAll("Kho hỗn hợp", mixedRepository.GetAll());

        var mixedService = new AllocationService<TeachingDevice>(
            mixedRepository, new HighestPriorityStrategy<TeachingDevice>(d => d.GetDefaultPriorityScore()));
        TeachingDevice bestMixed = mixedService.Allocate();
        Console.WriteLine($"   Điểm cao nhất trong kho hỗn hợp → {bestMixed.Id}");
        Check("AllocationService<TeachingDevice> chọn X-MC (điểm 95) mà không cần kiểm tra kiểu",
            bestMixed.Id == "X-MC");
        Check("Remove(\"X-AC\") trả về true", mixedRepository.Remove("X-AC"));
        Check("Remove(\"X-AC\") lần hai trả về false", !mixedRepository.Remove("X-AC"));

        // ---------- Tổng kết ----------
        Console.WriteLine();
        Console.ForegroundColor = failed == 0 ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"===== KẾT QUẢ: {passed} PASS, {failed} FAIL =====");
        Console.ResetColor();
    }

    // ================= Helpers =================

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== {title} ===");
        Console.ResetColor();
    }

    private static void Check(string description, bool condition)
    {
        if (condition)
        {
            passed++;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("   [PASS] ");
        }
        else
        {
            failed++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("   [FAIL] ");
        }

        Console.ResetColor();
        Console.WriteLine(description);
    }

    private static TException? ExpectThrows<TException>(string description, Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException ex)
        {
            Check($"{description}  ({ex.GetType().Name}: {ex.Message})", true);
            return ex;
        }
        catch (Exception ex)
        {
            Check($"{description}  (sai loại exception: {ex.GetType().Name})", false);
            return null;
        }

        Check($"{description}  (không có exception)", false);
        return null;
    }

    private static void PrintAll<T>(string title, IReadOnlyList<T> devices)
        where T : TeachingDevice
    {
        Console.WriteLine($"   {title}:");
        foreach (T device in devices)
        {
            Console.WriteLine($"     - {device}");
        }
    }

    private static string Ids<T>(IEnumerable<T> devices)
        where T : IIdentifiable
        => string.Join(", ", devices.Select(d => d.Id));
}
