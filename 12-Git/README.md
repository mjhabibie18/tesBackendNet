# 🌿 12 — Git

## Git untuk Backend Developer

Git adalah sistem version control yang wajib dikuasai setiap developer.

---

## Perintah Git Paling Penting

```bash
# Setup
git config --global user.name "Nama Kamu"
git config --global user.email "email@example.com"

# Inisialisasi
git init
git clone https://github.com/user/repo.git

# Status & diff
git status
git diff
git diff --staged

# Staging & commit
git add .
git add file.txt
git commit -m "feat: tambah fitur CRUD product"
git commit --amend  # edit commit terakhir

# Branch
git branch                  # lihat semua branch
git branch feature/login    # buat branch baru
git checkout feature/login  # pindah ke branch
git checkout -b feature/auth # buat dan pindah sekaligus
git merge feature/login     # merge branch
git branch -d feature/login # hapus branch

# Remote
git remote add origin https://github.com/...
git push origin main
git pull origin main
git fetch

# Stash
git stash           # simpan perubahan sementara
git stash pop       # kembalikan perubahan
git stash list      # lihat semua stash

# Log
git log --oneline
git log --graph --oneline
```

---

## Conventional Commits

Format pesan commit yang standar:

```
<type>(<scope>): <description>

feat:     Fitur baru
fix:      Bug fix
docs:     Perubahan dokumentasi
style:    Format code (tidak mengubah logika)
refactor: Refactoring
test:     Tambah/edit test
chore:    Build, config, dll
perf:     Performance improvement

Contoh:
feat(auth): tambah JWT refresh token
fix(crud): perbaiki bug soft delete
docs(readme): update cara instalasi
```

---

## Git Flow

```
main           → Production-ready code
develop        → Integration branch
feature/xxx    → Fitur baru
hotfix/xxx     → Fix urgent di production
release/xxx    → Persiapan rilis
```

---

## .gitignore Penting untuk .NET

```
bin/
obj/
*.user
.vs/
.idea/
.env
appsettings.Production.json
*.mdf
*.ldf
```

---

## 🎤 Tips Interview

**Q: "Apa bedanya git merge dan git rebase?"**
```
merge:  gabungkan dua branch dengan merge commit (history terjaga)
rebase: pindahkan commit ke base branch lain (history linear)
→ Untuk kolaborasi tim: gunakan merge
→ Untuk cleanup lokal: gunakan rebase
```

**Q: "Apa bedanya git fetch dan git pull?"**
```
fetch: download perubahan tapi TIDAK merge ke branch lokal
pull:  fetch + merge (download dan langsung merge)
```
