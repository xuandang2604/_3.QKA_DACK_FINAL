using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using _3.QKA_DACK.Models.CartModels;
using _3.QKA_DACK.Models.ProductModels;
using _3.QKA_DACK.Models.CategoryModels;
using _3.QKA_DACK.Models.Another;
using _3.QKA_DACK.Repositories.CartRepo;
using _3.QKA_DACK.Repositories.CategoryRepo;
using _3.QKA_DACK.Repositories.ProductRepo;
using _3.QKA_DACK.Repositories.ReviewRepo;
using _3.QKA_DACK.Repositories.BrandRepo;
namespace _3.QKA_DACK.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/products")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;

        private readonly IReviewRepository _reviewRepository;
        private readonly IBrandRepository _brandRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository,
                                UserManager<ApplicationUser> userManager, ICartRepository cartRepository, IReviewRepository reviewRepository, IBrandRepository brandRepository
                                )
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _cartRepository = cartRepository;
            _reviewRepository = reviewRepository;
            _brandRepository = brandRepository;
           
        }


        //[AllowAnonymous]
        //public async Task<IActionResult> Index(string searchString, string sortOrder)
        //{
        //    var products = await _productRepository.GetAllAsync();
        //    var categories = await _categoryRepository.GetAllAsync() ?? new List<Category>(); // Lấy danh mục
        //    ViewBag.Categories = categories;
        //    products ??= new List<Product>();
        //    // 🟢 Lọc theo danh mục (nếu có)
        //    //if (categoryId.HasValue)
        //    //{
        //    //    products = products.Where(p => p.CategoryId == categoryId);
        //    //}

        //    // 🟢 Lọc theo tên sản phẩm
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        //    }

        //    // 🟢 Sắp xếp theo giá
        //    products = sortOrder switch
        //    {
        //        "price_asc" => products.OrderBy(p => p.Price),
        //        "price_desc" => products.OrderByDescending(p => p.Price),
        //        _ => products
        //    };
        //    return View(products);

        //}

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoryId, string searchString, string sortOrder)
        {
            // Lấy danh sách danh mục từ cơ sở dữ liệu
            var categories = await _categoryRepository.GetAllAsync() ?? new List<Category>();

            // Tìm tất cả danh mục cha (ParentCategory == NULL)
            ViewBag.ParentCategories = categories.Where(c => c.ParentCategory == null).ToList();

            // Lấy danh mục con nếu có categoryId
            if (categoryId.HasValue)
            {
                ViewBag.ChildCategories = categories.Where(c => c.ParentCategoryId == categoryId.Value).ToList();
            }
            else
            {
                ViewBag.ChildCategories = new List<Category>();
            }

            // Lấy danh sách sản phẩm từ cơ sở dữ liệu
            var products = await _productRepository.GetAllAsync() ?? new List<Product>();

            // 🟢 Lọc sản phẩm theo danh mục được chọn (bao gồm danh mục cha và danh mục con)
            if (categoryId.HasValue)
            {
                // Tìm tất cả danh mục con của danh mục cha
                var selectedCategoryIds = GetAllChildCategoryIds(categoryId.Value, categories.ToList());

                // Lọc sản phẩm theo danh mục cha và con
                products = products.Where(p => selectedCategoryIds.Contains((int)p.CategoryId)).ToList();
            }

            // 🟢 Tìm kiếm sản phẩm theo tên (trong phạm vi danh mục đã lọc)
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 🟢 Sắp xếp sản phẩm theo giá (trong phạm vi danh mục đã lọc và tìm kiếm)
            products = sortOrder switch
            {
                "price_asc" => products.OrderBy(p => p.Price).ToList(),
                "price_desc" => products.OrderByDescending(p => p.Price).ToList(),
                _ => products.ToList()
            };

            return View(products);
        }

        private List<int> GetAllChildCategoryIds(int parentId, List<Category> categories)
        {
            // Lấy danh mục con của danh mục cha
            var childCategories = categories.Where(c => c.ParentCategoryId == parentId).ToList();

            // Khởi tạo danh sách ID danh mục (bao gồm cha và con)
            var categoryIds = new List<int> { parentId }; // Bắt đầu với danh mục cha

            foreach (var category in childCategories)
            {
                categoryIds.AddRange(GetAllChildCategoryIds(category.Id, categories));
            }

            return categoryIds; // Trả về danh sách ID của cha và con
        }













        // Hiển thị form thêm sản phẩm mới
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("add")]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var brands = await _brandRepository.GetAll();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Brand = new SelectList(brands, "Id", "Name");
            return View();
        }
        [Authorize(Roles = "Admin,Employee")]
        // Xử lý thêm sản phẩm mới
        [HttpPost("add")]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            var brands = await _brandRepository.GetAll();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Brand = new SelectList(brands, "Id", "Name");
            return View(product);
        }

        // Viết thêm hàm SaveImage để lưu hình ảnh
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }

        // Hiển thị chi tiết sản phẩm
        //[HttpGet("display/{id}")]
        //public async Task<IActionResult> Display(int id)
        //{
        //    var product = await _productRepository.GetByIdAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}
        //




        //Review And Comment
        [HttpGet("display/{id}")]
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var reviews = await _reviewRepository.GetReviewsByProductIdAsync(id);  // Get all reviews for the product

            var viewModel = new ProductDisplayViewModel
            {
                Product = product,
                Reviews = reviews
            };

            return View(viewModel);
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, string comment, int rating)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.Now
            };

            await _reviewRepository.AddAsync(review);
            return RedirectToAction("Display", new { id = productId });
        }








        // Hiển thị form cập nhật sản phẩm
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [HttpGet("update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost("update/{id}")]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.BrandId = product.BrandId; // Thêm dòng này nếu bạn có thuộc tính BrandId

                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var brands = await _brandRepository.GetAll();
            ViewBag.Brand = new SelectList(brands, "Id", "Name");
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        // Hiển thị form xác nhận xóa sản phẩm
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost("delete/{id}"), ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("addtocart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
                await _cartRepository.AddCartAsync(cart);
            }

            // Tiếp tục xử lý thêm sản phẩm...
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem { ProductId = productId, Quantity = quantity };
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _cartRepository.SaveChangesAsync();
            return RedirectToAction("Index", "Product", new { area = "Admin" });
        }
    }
}
