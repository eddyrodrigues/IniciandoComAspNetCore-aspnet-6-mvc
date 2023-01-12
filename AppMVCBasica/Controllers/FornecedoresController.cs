using AppMVCBasica.Data;
using AppMVCBasica.Extensions;
using AppMVCBasica.Models;
using AppMVCBasica.Notificacoes;
using AppMVCBasica.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppMVCBasica.Controllers;

[Authorize]
public class FornecedoresController : OwnController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFornecedorService _fornecedorService;

    public FornecedoresController(ApplicationDbContext context, IMapper mapper,
            IFornecedorService fornecedorService,
            INotificador notificador) : base(notificador)
    {
        _context = context;
        _mapper = mapper;
        _fornecedorService = fornecedorService;
    }

    // GET: Fornecedores
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {

        if (OperacaoEhValida())
        {
            return View();
        }


        return View(_mapper.Map<List<FornecedorViewModel>>(await _context.Fornecedores.ToListAsync()));
    }

    [AllowAnonymous]
    // GET: Fornecedores/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null || _context.Fornecedores == null)
        {
            return NotFound();
        }

        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(m => m.Id == id);
        if (fornecedor == null)
        {
            return NotFound();
        }

        return View(_mapper.Map<FornecedorViewModel>(fornecedor));
    }

    [ClaimsAuthorize("Fornecedor", "Adicionar")]
    // GET: Fornecedores/Create
    public async Task<IActionResult> CreateAsync()
    {
        var fornecedor = await PopulaFornecedor(new FornecedorViewModel());

        return View(fornecedor);
    }

    private Task<FornecedorViewModel> PopulaFornecedor(FornecedorViewModel fornecedorViewModel)
    {
        fornecedorViewModel.Endereco = new EnderecoViewModel();
        return Task.FromResult(fornecedorViewModel);
    }

    // POST: Fornecedores/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ClaimsAuthorize("Fornecedor", "Adicionar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FornecedorViewModel fornecedor)
    {
        if (ModelState.IsValid)
        {
            var fornecedorNovo = _mapper.Map<Fornecedor>(fornecedor);
            _context.Add(fornecedorNovo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(fornecedor);
    }

    // GET: Fornecedores/Edit/5
    [ClaimsAuthorize("Fornecedor", "Editar")]
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null || _context.Fornecedores == null)
        {
            return NotFound();
        }

        var fornecedor = await _context.Fornecedores.FindAsync(id);
        if (fornecedor == null)
        {
            return NotFound();
        }
        return View(_mapper.Map<FornecedorViewModel>(fornecedor));
    }

    // POST: Fornecedores/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ClaimsAuthorize("Fornecedor", "Editar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, FornecedorViewModel fornecedor)
    {
        if (id != fornecedor.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var fornecedorNovo = _mapper.Map<Fornecedor>(fornecedor);
                _context.Update(fornecedorNovo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FornecedorExists(fornecedor.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(fornecedor);
    }

    // GET: Fornecedores/Delete/5
    [ClaimsAuthorize("Fornecedor", "Excluir")]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null || _context.Fornecedores == null)
        {
            return NotFound();
        }

        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(m => m.Id == id);
        if (fornecedor == null)
        {
            return NotFound();
        }

        return View(fornecedor);
    }

    // POST: Fornecedores/Delete/5
    [HttpPost, ActionName("Delete")]
    [ClaimsAuthorize("Fornecedor", "Excluir")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (_context.Fornecedores == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Fornecedores'  is null.");
        }
        var fornecedor = await _context.Fornecedores.FindAsync(id);
        if (fornecedor != null)
        {
            _context.Fornecedores.Remove(fornecedor);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FornecedorExists(Guid id)
    {
      return _context.Fornecedores.Any(e => e.Id == id);
    }
}
