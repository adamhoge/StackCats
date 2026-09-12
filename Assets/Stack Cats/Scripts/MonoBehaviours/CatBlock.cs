using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void CatBlockRemoved(CatBlock catBlock);

    public class CatBlock : BlockComponent
    {
        public event CatBlockRemoved onCatBlockRemoved;

        public string CatBlockId;
        public SpriteRenderer BlockSpriteRenderer;
        public SpriteRenderer CatPawSpriteRenderer;
        public Sprite UnopenedCatPawSprite;
        public Sprite OpenedCatPawSprite;
        public CatBlockDestructionEffect DestructionEffect;

        public Cat Cat { get { return _cat; } set { SetCat(value); } }

        public bool IsSpecial { get { return _isSpecial; } set { SetIsSpecial(value); } }

        public bool IsOpened { get { return _isOpened; } set { SetIsOpened(value); } }

        public int Yarn;

        [SerializeField]
        private Cat _cat;

        [SerializeField]
        private bool _isSpecial;

        [SerializeField]
        private bool _isOpened;

        private Color _originalColor;
        private Color _originalCatPawColor;

        public override void OnStackChanged()
        {
            base.OnStackChanged();

            if (Block.GetBlocksAbove().Count == 0)
            {
                if (onCatBlockRemoved != null) onCatBlockRemoved(this);
                Block.ParentStack.RemoveBlock(Block, true);
            }
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();

            CatBlockDestructionEffect destructionEffect = Instantiate(DestructionEffect, transform.parent);
            destructionEffect.transform.position = transform.position;
            Color blockColor = _isSpecial ? HSBColor.ToColor(new HSBColor(((Time.time - (transform.position.x + transform.position.y / 2) / 4) * 0.5f) % 1, 0.65f, 1)) : _originalColor;
            Color catColor = _isSpecial ? HSBColor.ToColor(new HSBColor(((Time.time + 0.25f - (transform.position.x + transform.position.y / 2) / 4) * 0.5f) % 1, 0.15f, 1)) : _originalCatPawColor;
            destructionEffect.CatPawSpriteRenderer.sprite = _isOpened ? OpenedCatPawSprite : UnopenedCatPawSprite;
            destructionEffect.BlockSpriteRenderer.color = blockColor;
            destructionEffect.CatPawSpriteRenderer.color = catColor;
        }

        protected override void Awake()
        {
            base.Awake();

            _originalColor = BlockSpriteRenderer.color;
            _originalCatPawColor = CatPawSpriteRenderer.color;
        }

        protected void Update()
        {
            if (_isSpecial)
            {
                Color blockColor = _isSpecial ? HSBColor.ToColor(new HSBColor(((Time.time - (transform.position.x + transform.position.y / 2) / 4) * 0.35f) % 1, 0.65f, 1)) : _originalColor;
                Color catColor = _isSpecial ? HSBColor.ToColor(new HSBColor(((Time.time + 0.25f - (transform.position.x + transform.position.y / 2) / 4) * 0.35f) % 1, 0.15f, 1)) : _originalCatPawColor;
                BlockSpriteRenderer.color = blockColor;
                CatPawSpriteRenderer.color = catColor;
            }
        }

        private void SetCat(Cat cat)
        {
            if (_cat != cat)
            {
                _cat = cat;
            }
        }

        private void SetIsSpecial(bool value)
        {
            if (value == _isSpecial) return;

            _isSpecial = value;
        }

        private void SetIsOpened(bool value)
        {
            if (value == _isOpened) return;

            _isOpened = value;

            CatPawSpriteRenderer.sprite = _isOpened ? OpenedCatPawSprite : UnopenedCatPawSprite;
        }
    }
}